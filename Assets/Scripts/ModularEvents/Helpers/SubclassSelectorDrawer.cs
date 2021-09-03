using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomPropertyDrawer(typeof(SubclassSelectorAttribute))]
public class SubclassSelectorDrawer : PropertyDrawer
{

    bool initializeFold = false;

    List<System.Type> reflectionType;


    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)

    {

        if (property.propertyType != SerializedPropertyType.ManagedReference) return;


        SubclassSelectorAttribute utility = (SubclassSelectorAttribute)attribute;

        LazyGetAllInheritedType(utility.GetFieldType());

        Rect popupPosition = GetPopupPosition(position);


        string[] typePopupNameArray = reflectionType.Select(type => type == null ? "<null>" : type.ToString()).ToArray();

        string[] typeFullNameArray = reflectionType.Select(type => type == null ? "" : string.Format("{0} {1}", type.Assembly.ToString().Split(',')[0], type.FullName)).ToArray();


        //Get the type of serialized object 

        int currentTypeIndex = Array.IndexOf(typeFullNameArray, property.managedReferenceFullTypename);

        Type currentObjectType = reflectionType[currentTypeIndex];


        int selectedTypeIndex = EditorGUI.Popup(popupPosition, currentTypeIndex, typePopupNameArray);

        if (selectedTypeIndex >= 0 && selectedTypeIndex < reflectionType.Count)
        {

            if (currentObjectType != reflectionType[selectedTypeIndex])
            {

                if (reflectionType[selectedTypeIndex] == null)

                    //bug? NullReferenceException occurs when put null 

                    property.managedReferenceValue = null;

                else

                    property.managedReferenceValue = Activator.CreateInstance(reflectionType[selectedTypeIndex]);

                currentObjectType = reflectionType[selectedTypeIndex];

            }

        }

        if (initializeFold == false)
        {

            property.isExpanded = false;

            initializeFold = true;

        }

        EditorGUI.PropertyField(position, property, label, true);

    }


    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)

    {

        return EditorGUI.GetPropertyHeight(property, true);

    }


    void LazyGetAllInheritedType(System.Type baseType)

    {

        if (reflectionType != null) return;


        reflectionType = AppDomain.CurrentDomain.GetAssemblies()

            .SelectMany(s => s.GetTypes())

            .Where(p => baseType.IsAssignableFrom(p) && p.IsClass)

            .ToList();

        reflectionType.Insert(0, null);

    }


    Rect GetPopupPosition(Rect currentPosition)

    {

        Rect popupPosition = new Rect(currentPosition);

        popupPosition.width -= EditorGUIUtility.labelWidth;

        popupPosition.x += EditorGUIUtility.labelWidth;

        popupPosition.height = EditorGUIUtility.singleLineHeight;

        return popupPosition;

    }
}
