using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public interface IIncidentCriteria
	{
		Type Type { get; }
		bool Evaluate(IIncidentContext context);
	}

    public class IncidentCriteria : IIncidentCriteria
    {
        private Type type;
        private static Dictionary<string, Type> properties;

        public Type Type => type;

        [ValueDropdown("GetPropertyNames"), OnValueChanged("SetPrimitiveType"), HorizontalGroup("Group 1")]
        public string propertyName;

        [ShowIfGroup("PropertyChosen"), HideReferenceObjectPicker]
        public ICriteriaEvaluator evaluator;

        public IncidentCriteria() { }

        public IncidentCriteria(Type contextType)
		{
            this.type = contextType;
            GetPropertyList();
		}

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

        private void GetPropertyList()
        {
            if (type != null)
            {
                var propertyInfo = type.GetProperties();
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

        private IEnumerable<string> GetPropertyNames()
        {
            return properties.Keys.ToList();
        }

        private void SetPrimitiveType()
        {
            type = properties[propertyName];

            if (type == typeof(int))
            {
                evaluator = new IntegerEvaluator(propertyName);
            }
            else if(type == typeof(float))
			{
                evaluator = new FloatEvaluator();
			}
            else if(type == typeof(bool))
			{
                evaluator = new BoolEvaluator();
			}
        }

        bool PropertyChosen => propertyName != null;
    }

    [System.Serializable]
    public class IncidentCriteriaContainer
    {
        public List<IIncidentCriteria> criteria;

        public IncidentCriteriaContainer(List<IIncidentCriteria> c)
        {
            criteria = c;
        }

        public bool Evaluate(IIncidentContext context)
        {
            foreach (var criterium in criteria)
            {
                if (!criterium.Evaluate(context))
                {
                    return false;
                }
            }

            return true;
        }
    }
}