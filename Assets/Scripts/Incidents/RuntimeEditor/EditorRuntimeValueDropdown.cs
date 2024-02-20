using Game.Debug;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine.UI;

namespace Game.Incidents
{
    public class EditorRuntimeValueDropdown : SerializedMonoBehaviour, IRuntimeEditorComponent
    {
        public TMP_Text title;
        public TMP_Dropdown dropdown;
        public Action onValueChanged;

        private Dictionary<string, object> pairs;
        private IRuntimeEditorCompatible parentInstance;
        private FieldInfo fieldInfo;
        private Type parentType;
        public void Initialize(Type type, FieldInfo fieldInfo, CustomAttributeData attributeData, IRuntimeEditorCompatible parentInstance)
        {
            title.text = fieldInfo.Name;
            this.parentInstance = parentInstance;
            this.fieldInfo = fieldInfo;
            this.parentType = type;

            MethodInfo valueGetterMethod = type.GetMethod(attributeData.ConstructorArguments.First().Value.ToString(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            var methodReturn = (IEnumerable)valueGetterMethod.Invoke(parentInstance, new object[0]);
            var asStrings = new List<string>();
            pairs = new Dictionary<string, object>();
            foreach(var r in methodReturn)
            {
                asStrings.Add(r.ToString());
                pairs.Add(r.ToString(), r);
            }
            dropdown.onValueChanged.AddListener(delegate { OnValueChanged(dropdown); });
            dropdown.ClearOptions();
            dropdown.AddOptions(asStrings);
            OnValueChanged(dropdown);
        }

        private void OnValueChanged(TMP_Dropdown dropdown)
        {
            var text = dropdown.options[dropdown.value].text;
            fieldInfo.SetValue(parentInstance, pairs[text]);
            onValueChanged?.Invoke();
        }
    }
}