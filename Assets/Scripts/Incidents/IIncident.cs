using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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

	[System.Serializable]
	public class Incident : IIncident
	{
		public Type ContextType { get; set; }
		public int Weight { get; set; }
		public IncidentCriteriaContainer Criteria { get; set; }

		public IncidentActionContainer Actions { get; set; }

		[JsonConstructor]
		public Incident(Type contextType, IncidentCriteriaContainer criteria, IncidentActionContainer actions, int weight)
		{
			ContextType = contextType;
			Criteria = criteria;
			Actions = actions;
			Weight = weight;
		}

		public Incident(Type contextType, List<IIncidentCriteria> criteria, IncidentActionContainer container, int weight = 5)
		{
			ContextType = contextType;
			Criteria = new IncidentCriteriaContainer(criteria);
			Actions = container;
			Weight = weight;
		}

		public bool PerformIncident(IIncidentContext context, ref IncidentReport report )
		{
			return Actions.PerformActions(context, ref report);
		}
	}
}