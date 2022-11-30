using System;

namespace Game.Incidents
{
	public interface IIncident
	{
		string IncidentName { get; set; }
		Type ContextType { get; }
		int Weight { get; }
		IncidentCriteriaContainer Criteria { get; }
		IncidentActionHandlerContainer ActionContainer { get; }
		bool PerformIncident(IIncidentContext context, ref IncidentReport report);
	}
}