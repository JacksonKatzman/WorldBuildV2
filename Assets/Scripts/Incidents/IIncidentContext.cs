using Game.Factions;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface IIncidentContext
	{
		IIncidentContextProvider Provider { get; }
		Type ContextType { get; }
		int NumIncidents { get; }
	}

	public class FactionContext : IIncidentContext
	{
		public IIncidentContextProvider Provider { get; set; }
		public Type ContextType => typeof(FactionContext);
		public int NumIncidents { get; set; }
		public int Population { get; set; }
		//public int NumCities { get; set; }
		public float GooPercentage { get; set; }
		public bool IsFun { get; set; }
	}

	public class IncidentActionContainer
	{
		public List<IIncidentAction> Actions { get; set; }

		public IncidentActionContainer(List<IIncidentAction> actions)
		{
			Actions = actions;
		}

		public void PerformActions(IIncidentContext context)
		{
			foreach(var action in Actions)
			{
				action.PerformAction(context);
			}
		}
	}


	public interface IIncident
	{
		Type ContextType { get; }
		int Weight { get; }
		IncidentCriteriaContainer Criteria { get; }
		IncidentActionContainer Actions { get; }
	}

	[System.Serializable]
	public class Incident : IIncident
	{
		public Type ContextType  { get; set; }
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
	}
}