using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	[System.Serializable]
	public class Incident : IIncident
	{
		public Type ContextType { get; set; }
		public int Weight { get; set; }
		public IncidentCriteriaContainer Criteria { get; set; }

		public IncidentActionHandler ActionHandler { get; set; }

		[JsonConstructor]
		public Incident(Type contextType, IncidentCriteriaContainer criteria, IncidentActionHandler actions, int weight)
		{
			ContextType = contextType;
			Criteria = criteria;
			ActionHandler = actions;
			Weight = weight;
		}

		public Incident(Type contextType, List<IIncidentCriteria> criteria, IncidentActionHandler container, int weight = 5)
		{
			ContextType = contextType;
			Criteria = new IncidentCriteriaContainer(criteria);
			ActionHandler = container;
			Weight = weight;
		}

		public bool PerformIncident(IIncidentContext context, ref IncidentReport report )
		{
			var contextDictionary = new Dictionary<string, IIncidentContext>();
			contextDictionary.Add("{0}", context);
			report.Contexts = contextDictionary;

			if(!ActionHandler.VerifyActions(context))
			{
				return false;
			}

			ActionHandler.PerformActions(context, ref report);

			return true;
		}
	}
}