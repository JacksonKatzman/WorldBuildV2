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
        private Dictionary<string, Type> properties;

        [ValueDropdown("GetPropertyNames"), OnValueChanged("SetPrimitiveType"), HorizontalGroup("Group 1")]
        public string propertyName;

        [ShowIfGroup("PropertyChosen"), HideReferenceObjectPicker]
        public ICriteriaEvaluator evaluator;

        private bool PropertyChosen => propertyName != null;

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

        virtual public bool Evaluate(IIncidentContext context, IIncidentContext parentContext = null)
        {
            return evaluator.Evaluate(context, propertyName, parentContext);
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

                var validProperties = propertyInfo.Where(x => !interfacePropertyInfo.Any(y => x.Name == y.Name) && IsValidPropertyType(x.PropertyType)).ToList();

                properties.Clear();

                validProperties.ForEach(x => properties.Add(x.Name, x.PropertyType));
            }
        }

        virtual protected bool IsValidPropertyType(Type type)
		{
            return type == typeof(int) || type == typeof(float) || type == typeof(bool)
                || type == typeof(Dictionary<IIncidentContext, int>)
                || type == typeof(Dictionary<IIncidentContext, float>)
                || type == typeof(Dictionary<IIncidentContext, bool>)
                || type == typeof(List<IIncidentContext>);
                //|| (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>) && typeof(IIncidentContext).IsAssignableFrom(type.GetGenericArguments()[0]));
		}

        private IEnumerable<string> GetPropertyNames()
        {
            if(properties == null || properties.Count == 0)
			{
                GetPropertyList();
			}
            return properties.Keys.ToList();
        }

        virtual protected void SetPrimitiveType()
        {
            PrimitiveType = properties[propertyName];

            if (PrimitiveType == typeof(int))
            {
                evaluator = new IntegerEvaluator(propertyName, ContextType);
            }
            else if(PrimitiveType == typeof(float))
			{
                evaluator = new FloatEvaluator(propertyName, ContextType);
			}
            else if(PrimitiveType == typeof(bool))
			{
                evaluator = new BoolEvaluator(propertyName, ContextType);
			}
            else if(PrimitiveType == typeof(Dictionary<IIncidentContext,int>))
			{
                OutputLogger.Log("Found Complex Type");
                evaluator = new IntegerValueDictionaryEvaluator(propertyName, ContextType);
			}
            else if (PrimitiveType == typeof(Dictionary<IIncidentContext, float>))
            {
                OutputLogger.Log("Found Complex Type");
                evaluator = new FloatValueDictionaryEvaluator(propertyName, ContextType);
            }
            else if (PrimitiveType == typeof(Dictionary<IIncidentContext, bool>))
            {
                OutputLogger.Log("Found Complex Type");
                evaluator = new BoolValueDictionaryEvaluator(propertyName, ContextType);
            }
            else if (PrimitiveType == typeof(List<IIncidentContext>))
            {
                OutputLogger.Log("Found Complex Type");
                evaluator = new ListEvaluator(propertyName, ContextType);
            }
        }
    }
}