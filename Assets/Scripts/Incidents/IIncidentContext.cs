using Game.Factions;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public interface IIncidentContext
	{
		Type ContextType { get; }
		int NumIncidents { get; }
	}

	public class FactionContext : IIncidentContext
	{
		public Type ContextType => typeof(FactionContext);
		public int NumIncidents { get; set; }
		public int Population { get; set; }
		//public int NumCities { get; set; }
		public float GooPercentage { get; set; }
		public bool IsFun { get; set; }
	}

	public interface IIncidentAction
	{
		Type ContextType { get; }
		IIncidentContext Context { get; }
	}

	abstract public class IncidentAction<T> : IIncidentAction
	{
		private IIncidentContext context;
		public Type ContextType => typeof(T);
		public IIncidentContext Context
		{
			get { return context; }
			set
			{
				if(value.ContextType == ContextType)
				{
					context = value;
				}
			}
		}

		public void PerformDebugAction()
		{
			OutputLogger.Log("Debug Action Performed!");
		}
	}

	/// <summary>
	/// inherit from and then make abstract
	/// </summary>
	public class FactionIncidentAction : IncidentAction<FactionContext>
	{
	}
	

	public interface IIncident
	{
		Type ContextType { get; }
		int Weight { get; }
		IncidentCriteriaContainer Criteria { get; }
		List<IIncidentAction> Actions { get; }

		void PerformDebugAction();

	}

	public class Incident : IIncident
	{
		public Type ContextType  { get; set; }
		public int Weight { get; set; }
		public IncidentCriteriaContainer Criteria { get; set; }
		public List<IIncidentAction> Actions { get; set; }

		public Incident(Type contextType, List<IIncidentCriteria> criteria)
		{
			ContextType = contextType;
			Criteria = new IncidentCriteriaContainer(criteria);
		}

		public void PerformDebugAction()
		{
			OutputLogger.Log("Debug Action Performed!");
		}
	}

	public class IncidentService
	{
		private static IncidentService instance;

		private List<IIncident> incidents;

		public static IncidentService Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new IncidentService();
				}
				return instance;
			}
		}

		private IncidentService()
		{
			incidents = new List<IIncident>();

			var criteria = new List<IIncidentCriteria>();

			ICriteriaEvaluator evaluator = new IntegerEvaluator(">", 10);
			var criterium = new IncidentCriteria("Population", typeof(FactionContext), evaluator);
			criteria.Add(criterium);

			evaluator = new IntegerEvaluator("<", 20);
			criterium = new IncidentCriteria("Population", typeof(FactionContext), evaluator);
			criteria.Add(criterium);

			evaluator = new FloatEvaluator(">", 20);
			criterium = new IncidentCriteria("GooPercentage", typeof(FactionContext), evaluator);
			criteria.Add(criterium);

			evaluator = new BoolEvaluator("==", false);
			criterium = new IncidentCriteria("IsFun", typeof(FactionContext), evaluator);
			criteria.Add(criterium);

			var debugIncident = new Incident(typeof(FactionContext), criteria);
			incidents.Add(debugIncident);
		}

		public void PerformIncidents<T>(IIncidentContextProvider<T> incidentContextProvider) where T : IIncidentContext
		{
			var incidentsOfType = GetIncidentsOfType<T>();
			if(incidentsOfType == null || incidentsOfType.Count == 0)
			{
				return;
			}

			var incidentContext = incidentContextProvider.GetContext();

			var possibleIncidents = GetIncidentsWithMatchingCriteria(incidentsOfType, incidentContext);


			if(possibleIncidents == null || possibleIncidents.Count == 0)
			{
				OutputLogger.Log("No matching incidents!");
				return;
			}

			possibleIncidents.FirstOrDefault().PerformDebugAction();
		}

		private List<IIncident> GetIncidentsOfType<T>() where T : IIncidentContext
		{
			var items = incidents.Where(x => x.ContextType == typeof(T)).ToList();
			return items;
		}
		private List<IIncident> GetIncidentsWithMatchingCriteria(List<IIncident> incidents, IIncidentContext context)
		{
			var items = incidents.Where(x => x.Criteria.Evaluate(context) == true).ToList();
			return items;
		}
	}

	public class EditableIncident : SerializedScriptableObject
	{
		public string incidentName;
		public int weight;
	}
}