using System;

namespace Game.Incidents
{
	public interface ICriteriaEvaluator
	{
        Type Type { get; }
        bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext = null);
		void Setup();
	}
}