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

		public IncidentActionHandlerContainer ActionContainer { get; set; }

		[JsonConstructor]
		public Incident(Type contextType, IncidentCriteriaContainer criteria, IncidentActionHandlerContainer actions, int weight)
		{
			ContextType = contextType;
			Criteria = criteria;
			ActionContainer = actions;
			Weight = weight;
		}

		public Incident(Type contextType, List<IIncidentCriteria> criteria, IncidentActionHandlerContainer container, int weight = 5)
		{
			ContextType = contextType;
			Criteria = new IncidentCriteriaContainer(criteria);
			ActionContainer = container;
			Weight = weight;
		}

		public bool PerformIncident(IIncidentContext context, ref IncidentReport report )
		{
			var contextDictionary = new Dictionary<string, IIncidentContext>();
			contextDictionary.Add("{0}", context);
			report.Contexts = contextDictionary;

			if(!ActionContainer.VerifyActions(context))
			{
				return false;
			}

			ActionContainer.PerformActions(context, ref report);

			return true;
		}
	}
}