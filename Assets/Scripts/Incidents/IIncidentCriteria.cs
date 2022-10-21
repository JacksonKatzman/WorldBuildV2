using System;

namespace Game.Incidents
{
	public interface IIncidentCriteria
	{
		Type ContextType { get; }
		bool Evaluate(IIncidentContext context);
	}
}