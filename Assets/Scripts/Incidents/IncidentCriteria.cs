using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class IncidentCriteria : IIncidentCriteria
    {
        public Type ContextType { get; set; }
        public Type PrimitiveType { get; set; }
        private static Dictionary<string, Type> properties;

        [ValueDropdown("GetPropertyNames"), OnValueChanged("SetPrimitiveType"), HorizontalGroup("Group 1")]
        public string propertyName;

        [ShowIfGroup("PropertyChosen"), HideReferenceObjectPicker]
        public ICriteriaEvaluator evaluator;

        public IncidentCriteria() 
        {
            GetPropertyList();
        }

        public IncidentCriteria(Type contextType)
		{
            ContextType = contextType;
            GetPropertyList();
		}

        public IncidentCriteria(string propertyName, Type type, ICriteriaEvaluator evaluator)
        {
            this.propertyName = propertyName;
            ContextType = type;
            this.evaluator = evaluator;
        }

        public bool Evaluate(IIncidentContext context)
        {
            return evaluator.Evaluate(context, propertyName);
        }

        private void GetPropertyList()
        {
            if (properties == null)
            {
                properties = new Dictionary<string, Type>();
            }
            if (ContextType != null)
            {
                var propertyInfo = ContextType.GetProperties();
                var interfacePropertyInfo = typeof(IIncidentContext).GetProperties();

                var validProperties = propertyInfo.Where(x => !interfacePropertyInfo.Any(y => x.Name == y.Name)).ToList();

                properties.Clear();

                validProperties.ForEach(x => properties.Add(x.Name, x.PropertyType));
            }
        }

        private IEnumerable<string> GetPropertyNames()
        {
            if(properties == null || properties.Count == 0)
			{
                GetPropertyList();
			}
            return properties.Keys.ToList();
        }

        private void SetPrimitiveType()
        {
            PrimitiveType = properties[propertyName];

            if (PrimitiveType == typeof(int))
            {
                evaluator = new IntegerEvaluator(propertyName);
            }
            else if(PrimitiveType == typeof(float))
			{
                evaluator = new FloatEvaluator(propertyName);
			}
            else if(PrimitiveType == typeof(bool))
			{
                evaluator = new BoolEvaluator(propertyName);
			}
        }

        bool PropertyChosen => propertyName != null;
    }
}