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

    public class IncidentCriteria : IIncidentCriteria
	{
        private Type type;

        public Type Type => type;

        [ValueDropdown("GetPropertyNames"), OnValueChanged("SetPrimitiveType")]
        public string propertyName;

        [ShowIfGroup("PropertyChosen")]
        public ICriteriaEvaluator evaluator;

        public IncidentCriteria(string propertyName, Type type, ICriteriaEvaluator evaluator)
		{
            this.propertyName = propertyName;
            this.type = type;
            this.evaluator = evaluator;
		}

        public bool Evaluate(IIncidentContext context)
		{
            return evaluator.Evaluate(context, propertyName);
		}

        private IEnumerable<string> GetPropertyNames()
		{
            return IncidentEditorWindow.Properties.Keys.ToList();
		}

        private void SetPrimitiveType()
		{
            type = IncidentEditorWindow.Properties[propertyName];

            if(type == typeof(int))
			{
                evaluator = new IntegerEvaluator();
			}
		}

        bool PropertyChosen => type != null;
    }

    public interface ICriteriaEvaluator
	{
        Type Type { get; }
        bool Evaluate(IIncidentContext context, string propertyName);
	}

    public class IntegerEvaluator : ICriteriaEvaluator
	{
        private static Dictionary<string, Func<int, int, bool>> comparators = new Dictionary<string, Func<int, int, bool>>
        {
            {">", (a, b) => a > b },
            {">=", (a, b) => a >= b },
            {"<", (a, b) => a < b },
            {"<=", (a, b) => a <= b },
            {"==", (a, b) => a == b },
            {"!=", (a, b) => a != b },
        };

        [HideInInspector]
        public Type Type => typeof(int);

        private string comparator;

        [ValueDropdown("GetComparatorNames"), OnValueChanged("SetComparatorType")]
        public string Comparator;

        public int value;

        public IntegerEvaluator() { }

        public IntegerEvaluator(string operation, int value)
		{
            comparator = operation;
            this.value = value;
		}

		public bool Evaluate(IIncidentContext context, string propertyName)
		{
            var propertyValue = (int)context.GetType().GetProperty(propertyName).GetValue(context);
            return comparators[comparator].Invoke(propertyValue, value);
		}

        private List<string> GetComparatorNames()
		{
            return comparators.Keys.ToList();
		}

        private void SetComparatorType()
		{
            comparator = Comparator;
		}
	}
}