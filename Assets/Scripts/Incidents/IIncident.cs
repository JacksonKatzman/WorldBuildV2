using System;

namespace Game.Incidents
{
	public interface IIncident
	{
		Type ContextType { get; }
		int Weight { get; }
		IncidentCriteriaContainer Criteria { get; }
		IncidentActionContainer Actions { get; }
		bool PerformIncident(IIncidentContext context, ref IncidentReport report);
	}
}