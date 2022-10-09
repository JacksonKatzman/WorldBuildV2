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

        public Type Type => type;

        [ValueDropdown("GetPropertyNames"), OnValueChanged("SetPrimitiveType"), HorizontalGroup("Group 1")]
        public string propertyName;

        [ShowIfGroup("PropertyChosen"), HideReferenceObjectPicker]
        public ICriteriaEvaluator evaluator;

        public IncidentCriteria()
		{

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

        private IEnumerable<string> GetPropertyNames()
        {
            return IncidentEditorWindow.Properties.Keys.ToList();
        }

        private void SetPrimitiveType()
        {
            type = IncidentEditorWindow.Properties[propertyName];

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

        bool PropertyChosen => type != null;
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