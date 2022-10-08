using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Game.Incidents
{
	public class IncidentEditorWindow : OdinEditorWindow
	{
		[MenuItem("World Builder/Incident Editor")]
		private static void OpenWindow()
		{
			GetWindow<IncidentEditorWindow>("Incident Editor");
		}

		protected override void OnGUI()
		{
			base.OnGUI();
		}

        private static Type contextType;
        private static Dictionary<string, Type> properties;

        public static Type ContextType
		{
            get { return contextType; }
            set {
                    contextType = value;
                    GetPropertyList();
                }
		}

        public static Dictionary<string, Type> Properties => properties;

		[TypeFilter("GetFilteredTypeList"), OnValueChanged("SetContextType")]
        public IIncidentContext A;

        [ShowIfGroup("ContextTypeChosen")]
        public List<IncidentCriteria> criteria;

        public IEnumerable<Type> GetFilteredTypeList()
        {
            var q = typeof(IIncidentContext).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)                                          // Excludes BaseClass
                .Where(x => !x.IsGenericTypeDefinition)                             // Excludes C1<>
                .Where(x => typeof(IIncidentContext).IsAssignableFrom(x));                 // Excludes classes not inheriting from BaseClass

            return q;
        }

        void SetContextType()
		{
            ContextType = A.ContextType;
            criteria = new List<IncidentCriteria>();
		}

        private static void GetPropertyList()
        {
            if (ContextType != null)
            {
                var propertyInfo = ContextType.GetProperties();
                var interfacePropertyInfo = typeof(IIncidentContext).GetProperties();

                var validProperties = propertyInfo.Where(x => !interfacePropertyInfo.Any(y => x.Name == y.Name)).ToList();

                if (properties == null)
                {
                    properties = new Dictionary<string, Type>();
                }
                properties.Clear();

                validProperties.ForEach(x => properties.Add(x.Name, x.PropertyType));
            }
        }

        public bool ContextTypeChosen => A != null;
    }
}