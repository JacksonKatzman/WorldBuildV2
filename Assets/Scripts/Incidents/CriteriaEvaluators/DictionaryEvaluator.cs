using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	abstract public class DictionaryEvaluator<T> : CriteriaEvaluator<T>
	{
        public DictionaryEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }
        override protected bool Evaluate(IIncidentContext context, string propertyName)
        {
            var propertyValue = (Dictionary<IIncidentContext, T>)context.GetType().GetProperty(propertyName).GetValue(context);

            return EvaluateByContains(propertyValue, context);
        }

        protected bool EvaluateByContains(Dictionary<IIncidentContext, T> dictionary, IIncidentContext context)
		{
            var combinedExpressions = CombineExpressions(context);

            foreach (var value in dictionary.Values)
            {
                if (Comparators[Comparator].Invoke(value, combinedExpressions))
                {
                    return true;
                }
            }
            return false;
        }
    }
}