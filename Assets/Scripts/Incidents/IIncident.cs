using System;

namespace Game.Incidents
{
	public interface IIncident
	{
		string IncidentName { get; set; }
		Type ContextType { get; }
		IIncidentWeight Weights { get; }
		IncidentCriteriaContainer Criteria { get; }
		IncidentCriteriaContainer WorldCriteria { get; }
		IncidentActionHandlerContainer ActionContainer { get; }
		public bool IsUnique { get; set; }
		bool PerformIncident(IIncidentContext context, ref IncidentReport report);
	}
}