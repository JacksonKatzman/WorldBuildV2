using Game.Debug;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Incidents
{
    public class EditorRuntimeBlock : SerializedMonoBehaviour, IRuntimeEditorComponent
    {
        public TMP_Text title;
        public Image background;
        public Transform Root => transform;
        public Type componentType;
        public IRuntimeEditorCompatible instance;
        public Dictionary<FieldInfo, IRuntimeEditorComponent> fieldComponents;
        public Color color = Color.white;

        public void Initialize(Type componentType, FieldInfo fieldInfo, IRuntimeEditorCompatible parent, Color parentColor)
        {
            title.text = fieldInfo.Name;
            color = parentColor == Color.white ? Color.gray : Color.white;
            background.color = color;

            this.componentType = componentType;
            //construct an instance of the type that we will mock with this ui
            instance = (IRuntimeEditorCompatible)Activator.CreateInstance(this.componentType);
            //set the field in the parent to be equal to this instance
            fieldInfo.SetValue(parent, instance);

            fieldComponents = new Dictionary<FieldInfo, IRuntimeEditorComponent>();

            if (RuntimeEditorUtilities.GetAllFieldsForType(componentType, out var fields))
            {
                foreach (var field in fields)
                {
                    if(typeof(IRuntimeEditorCompatible).IsAssignableFrom(field.FieldType))
                    {
                        //make a custom attribute to allow us to create using special types? for interfaces. TypeConstrcutorAttribute?
                        CreateNewBlock(field.FieldType, field, instance, Root);
                        continue;
                    }

                    if(FieldHasAttribute(field, typeof(Sirenix.OdinInspector.ValueDropdownAttribute), out var attribute))
                    {
                        var valueDropdown = Instantiate(RuntimeEditorPrefabs.Instance.valueDropdownPrefab, Root);
                        valueDropdown.Initialize(this.componentType, field, attribute, instance);

                        if(FieldHasAttribute(field, typeof(Sirenix.OdinInspector.OnValueChangedAttribute), out var valueChangedAttribute))
                        {
                            MethodInfo valueGetterMethod = componentType.GetMethod(valueChangedAttribute.ConstructorArguments.First().Value.ToString(), BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                            valueDropdown.onValueChanged += () => valueGetterMethod.Invoke(instance, null);
                        }
                    }

                    if(field.FieldType == typeof(string) || field.FieldType == typeof(int) || field.FieldType == typeof(float) || field.FieldType == typeof(bool))
                    {
                        var inputField = Instantiate(RuntimeEditorPrefabs.Instance.inputPrefab, Root);
                        inputField.Initialize(field, instance);
                    }
                }
            }
        }

        private EditorRuntimeBlock CreateNewBlock(Type componentType, FieldInfo fieldInfo, IRuntimeEditorCompatible parent, Transform parentTransform)
        {
            var block = Instantiate(RuntimeEditorPrefabs.Instance.blockPrefab, parentTransform);
            block.Initialize(componentType, fieldInfo, parent, color);
            return block;
        }

        private bool FieldHasAttribute(FieldInfo field, Type attributeType, out CustomAttributeData attributeData)
        {
            var customAttributes = field.CustomAttributes;
            var matchingAttributes = field.CustomAttributes.Where(x => x.AttributeType == attributeType);
            if (matchingAttributes.Count() == 1)
            {
                attributeData = matchingAttributes.First();
                return true;
            }

            attributeData = null;
            return false;
        }
    }
}