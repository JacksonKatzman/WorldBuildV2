using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Game.Incidents
{
    static public class RuntimeEditorUtilities
    {
        public static bool GetAllFieldsForType(Type type, out List<FieldInfo> fields, bool allowPrivate = false)
        {
            if (!typeof(IRuntimeEditorCompatible).IsAssignableFrom(type))
            {
                fields = null;
                return false;
            }

            if (allowPrivate)
            {
                fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();
            }
            else
            {
                fields = type.GetFields().ToList();
            }
            return true;
        }

        public static bool GetAllPropertiesForType(Type type, out List<PropertyInfo> properties, bool allowPrivate = false)
        {
            if (!typeof(IRuntimeEditorCompatible).IsAssignableFrom(type))
            {
                properties = null;
                return false;
            }

            if (allowPrivate)
            {
                properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).ToList();
            }
            else
            {
                properties = type.GetProperties().ToList();
            }
            return true;
        }
    }
}