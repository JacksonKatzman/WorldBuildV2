using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface ICriteriaEvaluator
	{
        Type Type { get; }
        bool Evaluate(IIncidentContext context, string propertyName);
	}

	abstract public class ContextEvaluator<T> : ICriteriaEvaluator where T : IIncidentContext
	{
		public Type Type => typeof(T);

		public bool Evaluate(IIncidentContext context, string propertyName)
		{
			contextField.CalculateField
			var propertyValue = (T)context.GetType().GetProperty(propertyName).GetValue(context);
			return 
		}
	}
}