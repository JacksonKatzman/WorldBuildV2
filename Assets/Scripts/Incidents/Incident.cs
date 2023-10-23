using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	[System.Serializable]
	public class Incident : IIncident
	{
		public string IncidentName { get; set; }
		public Type ContextType { get; set; }
		public IIncidentWeight Weights { get; set; }
		public IncidentCriteriaContainer Criteria { get; set; }
		public IncidentCriteriaContainer WorldCriteria { get; set; }
		public bool IsMajorIncident { get; set; }

		public IncidentActionHandlerContainer ActionContainer { get; set; }

		[JsonConstructor]
		public Incident(Type contextType, IncidentCriteriaContainer criteria, IncidentCriteriaContainer worldCriteria, IncidentActionHandlerContainer actions, IIncidentWeight weight, bool isMajorIncident)
		{
			ContextType = contextType;
			Criteria = criteria;
			WorldCriteria = worldCriteria;
			ActionContainer = actions;
			Weights = weight;
			IsMajorIncident = isMajorIncident;
			if(WorldCriteria == null)
			{
				WorldCriteria = new IncidentCriteriaContainer(new List<IIncidentCriteria>());
			}
		}

		public Incident(string incidentName, Type contextType, List<IIncidentCriteria> criteria, List<IIncidentCriteria> worldCriteria, IncidentActionHandlerContainer container, IIncidentWeight weight, bool isMajorIncident)
		{
			IncidentName = incidentName;
			ContextType = contextType;
			Criteria = new IncidentCriteriaContainer(criteria);
			WorldCriteria = new IncidentCriteriaContainer(worldCriteria);
			ActionContainer = container;
			Weights = weight;
			IsMajorIncident = isMajorIncident;
		}

		public bool PerformIncident(IIncidentContext context, ref IncidentReport report )
		{
			var contextDictionary = new Dictionary<string, IIncidentContext>();
			contextDictionary.Add("{0}", context);

			var matchingFields = ActionFieldReflection.GetGenericFieldsByType(ContextType, typeof(DeployedContextActionField<>));
			var startingValue = 1;
			foreach (var f in matchingFields)
			{
				var actionField = (IIncidentActionField)f.GetValue(context);
				contextDictionary.Add("{" + startingValue + "}", actionField.GetFieldValue());
				startingValue++;
			}
			report.Contexts = contextDictionary;
			report.IsMajorIncident = IsMajorIncident;

			if(!ActionContainer.VerifyActions(context))
			{
				return false;
			}

			ActionContainer.PerformActions(context, ref report);

			return true;
		}
	}
}