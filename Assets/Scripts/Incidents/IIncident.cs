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
		void PerformIncident(int incidentID, IIncidentContext context);
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

		public Incident(Type contextType, List<IIncidentCriteria> criteria, List<IIncidentAction> actions, int weight = 5)
		{
			ContextType = contextType;
			Criteria = new IncidentCriteriaContainer(criteria);
			Actions = new IncidentActionContainer(actions);
			Weight = weight;
		}

		public void PerformIncident(int incidentID, IIncidentContext context)
		{
			Actions.PerformActions(context);
		}
	}
}