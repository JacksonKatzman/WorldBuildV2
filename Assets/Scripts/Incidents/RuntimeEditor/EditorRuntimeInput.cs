using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Incidents
{
    public class EditorRuntimeInput : SerializedMonoBehaviour, IRuntimeEditorComponent
    {
        public TMP_Text title;
        public TMP_InputField inputField;
        public Slider slider;
        public TMP_Text sliderValueDisplay;
        public Toggle toggle;
        private IRuntimeEditorCompatible parentInstance;
        private FieldInfo fieldInfo;

        public void Initialize(FieldInfo fieldInfo, IRuntimeEditorCompatible parentInstance)
        {
            this.parentInstance = parentInstance;
            this.fieldInfo = fieldInfo;
            title.text = fieldInfo.Name;

            inputField.onEndEdit.AddListener(delegate { OnEndEdit(inputField); });
            slider.onValueChanged.AddListener(delegate { OnSliderValueChanged(slider); });
            toggle.onValueChanged.AddListener(delegate { OnToggleValueChanged(toggle); });

            var fieldType = fieldInfo.FieldType;
            if(fieldType == typeof(string))
            {
                var customAttributes = fieldInfo.CustomAttributes;
                var matchingAttributes = customAttributes.Where(x => x.AttributeType == typeof(UnityEngine.TextAreaAttribute));
                if (matchingAttributes.Count() == 1)
                {
                    var attribute = matchingAttributes.First();
                    var lower = Int32.Parse(attribute.ConstructorArguments[0].Value.ToString());
                    var upper = Int32.Parse(attribute.ConstructorArguments[1].Value.ToString());
                    var rect = inputField.GetComponent<RectTransform>();
                    rect.sizeDelta = new Vector2(rect.sizeDelta.x, 30 * lower);
                    inputField.lineLimit = 0;
                }
                inputField.gameObject.SetActive(true);
                OnEndEdit(inputField);
            }
            else if(fieldType == typeof(int) || fieldType == typeof(float))
            {
                var customAttributes = fieldInfo.CustomAttributes;
                var matchingAttributes = customAttributes.Where(x => x.AttributeType == typeof(UnityEngine.RangeAttribute));
                if (matchingAttributes.Count() == 1)
                {
                    var attribute = matchingAttributes.First();
                    var lower = attribute.ConstructorArguments[0].Value;
                    var upper = attribute.ConstructorArguments[1].Value;
                    
                    if(fieldType == typeof(float))
                    {
                        slider.minValue = float.Parse(lower.ToString());
                        slider.maxValue = float.Parse(upper.ToString());
                        slider.wholeNumbers = false;
                    }
                    else
                    {
                        slider.minValue = Int32.Parse(lower.ToString());
                        slider.maxValue = Int32.Parse(upper.ToString());
                        slider.wholeNumbers = true;
                    }
                    slider.gameObject.SetActive(true);
                    OnSliderValueChanged(slider);
                }
                else
                {
                    if (fieldType == typeof(float))
                    {
                        inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
                    }
                    else
                    {
                        inputField.contentType = TMP_InputField.ContentType.IntegerNumber;
                    }

                    inputField.gameObject.SetActive(true);
                    inputField.text = "0";
                    OnEndEdit(inputField);
                }
            }
            else if(fieldType == typeof(bool))
            {
                toggle.gameObject.SetActive(true);
                OnToggleValueChanged(toggle);
            }
        }

        private void OnEndEdit(TMP_InputField inputField)
        {
            //var text = dropdown.options[dropdown.value].text;
            //OutputLogger.Log($"Dropdown changed to: {pairs[text].ToString()} : {pairs[text].GetType()}");
            //fieldInfo.SetValue(parentInstance, pairs[text]);
            var value = inputField.text;
            if(inputField.contentType == TMP_InputField.ContentType.Standard)
            {
                fieldInfo.SetValue(parentInstance, value);
            }
            else if(inputField.contentType == TMP_InputField.ContentType.IntegerNumber)
            {
                fieldInfo.SetValue(parentInstance, Int32.Parse(value));
            }
            else if(inputField.contentType == TMP_InputField.ContentType.DecimalNumber)
            {
                fieldInfo.SetValue(parentInstance, float.Parse(value));
            }
        }

        private void OnSliderValueChanged(Slider slider)
        {
            var value = slider.value;
            if (inputField.contentType == TMP_InputField.ContentType.IntegerNumber)
            {
                fieldInfo.SetValue(parentInstance, (int)value);
            }
            else if (inputField.contentType == TMP_InputField.ContentType.DecimalNumber)
            {
                fieldInfo.SetValue(parentInstance, value);
            }
            sliderValueDisplay.text = value.ToString("N2");
        }

        private void OnToggleValueChanged(Toggle toggle)
        {
            fieldInfo.SetValue(parentInstance, toggle.isOn);
        }
    }
}