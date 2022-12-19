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

		public IncidentActionHandlerContainer ActionContainer { get; set; }

		[JsonConstructor]
		public Incident(Type contextType, IncidentCriteriaContainer criteria, IncidentActionHandlerContainer actions, IIncidentWeight weight)
		{
			ContextType = contextType;
			Criteria = criteria;
			ActionContainer = actions;
			Weights = weight;
			/*
			var dataType = new Type[] { ContextType };
			var genericBase = typeof(IncidentWeight<>);
			var combinedType = genericBase.MakeGenericType(dataType);
			Weights= (IIncidentWeight)Activator.CreateInstance(combinedType, weight);
			*/
		}

		public Incident(string incidentName, Type contextType, List<IIncidentCriteria> criteria, IncidentActionHandlerContainer container, IIncidentWeight weight)
		{
			IncidentName = incidentName;
			ContextType = contextType;
			Criteria = new IncidentCriteriaContainer(criteria);
			ActionContainer = container;
			Weights = weight;
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

			if(!ActionContainer.VerifyActions(context))
			{
				return false;
			}

			ActionContainer.PerformActions(context, ref report);

			return true;
		}
	}
}