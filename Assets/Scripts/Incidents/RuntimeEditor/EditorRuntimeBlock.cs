using Game.Debug;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Game.Incidents
{
    public class EditorRuntimeBlock : SerializedMonoBehaviour, IRuntimeEditorComponent
    {
        public Transform Root => transform;
        public Type componentType;
        public IRuntimeEditorCompatible instance;
        public Dictionary<FieldInfo, IRuntimeEditorComponent> fieldComponents;

        public void Initialize(FieldInfo fieldInfo, IRuntimeEditorCompatible parent)
        {
            this.componentType = fieldInfo.FieldType;
            instance = (IRuntimeEditorCompatible)Activator.CreateInstance(this.componentType);

            fieldInfo.SetValue(parent, instance);
            fieldComponents = new Dictionary<FieldInfo, IRuntimeEditorComponent>();

            if (RuntimeEditorUtilities.GetAllFieldsForType(componentType, out var fields))
            {
                foreach (var field in fields)
                {
                    if(typeof(IRuntimeEditorCompatible).IsAssignableFrom(field.FieldType))
                    {
                        CreateNewBlock(field, instance, Root);
                        continue;
                    }

                    var customAttributes = field.CustomAttributes;
                    var matchingAttributes = field.CustomAttributes.Where(x => x.AttributeType == typeof(Sirenix.OdinInspector.ValueDropdownAttribute));
                    if(matchingAttributes.Count() == 1)
                    {
                        var attribute = matchingAttributes.First();
                        var valueDropdown = Instantiate(RuntimeEditorPrefabs.Instance.valueDropdownPrefab, Root);
                        valueDropdown.Initialize(this.componentType, field, attribute, instance);
                        continue;
                    }


                }
            }
        }

        private EditorRuntimeBlock CreateNewBlock(FieldInfo fieldInfo, IRuntimeEditorCompatible parent, Transform parentTransform)
        {
            var block = Instantiate(RuntimeEditorPrefabs.Instance.blockPrefab, parentTransform);
            block.Initialize(fieldInfo, parent);
            return block;
        }
    }
}