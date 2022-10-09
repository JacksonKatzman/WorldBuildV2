using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using System;

namespace Game.Incidents
{
	public class IncidentService
	{
		public static string INCIDENT_DATA_PATH = "/Resources/IncidentData/";
		public static JsonSerializerSettings SERIALIZER_SETTINGS = new JsonSerializerSettings()
		{
			// ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			PreserveReferencesHandling = PreserveReferencesHandling.Objects,
			TypeNameHandling = TypeNameHandling.All
		};

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
			OutputLogger.Log("Calling Setup!");
			Setup();
		}

		public void PerformIncidents(IIncidentContextProvider incidentContextProvider)
		{
			var contextType = incidentContextProvider.GetContext().GetType();
			var incidentsOfType = GetIncidentsOfType(contextType);
			if(incidentsOfType == null || incidentsOfType.Count == 0)
			{
				OutputLogger.Log("No incidents of that type!");
				return;
			}

			var incidentContext = incidentContextProvider.GetContext();

			var possibleIncidents = GetIncidentsWithMatchingCriteria(incidentsOfType, incidentContext);


			if(possibleIncidents == null || possibleIncidents.Count == 0)
			{
				OutputLogger.Log("No matching incidents!");
				return;
			}

			possibleIncidents.FirstOrDefault().Actions.PerformActions(incidentContext);
		}

		private List<IIncident> GetIncidentsOfType(Type type)
		{
			var items = incidents.Where(x => x.ContextType == type).ToList();
			return items;
		}
		private List<IIncident> GetIncidentsWithMatchingCriteria(List<IIncident> incidents, IIncidentContext context)
		{
			var items = incidents.Where(x => x.Criteria.Evaluate(context) == true).ToList();
			return items;
		}

		private void Setup()
		{
			incidents = new List<IIncident>();
			var files = Directory.GetFiles(Application.dataPath + INCIDENT_DATA_PATH, "*.json");
			foreach (string file in files)
			{
				var text = File.ReadAllText(file);
				if (text.Length > 0)
				{
					var item = JsonConvert.DeserializeObject<Incident>(text, SERIALIZER_SETTINGS);
					incidents.Add(item);
				}
			}
		}

		private void DebugSetup()
		{
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

			evaluator = new BoolEvaluator("==", true);
			criterium = new IncidentCriteria("IsFun", typeof(FactionContext), evaluator);
			criteria.Add(criterium);

			var actions = new List<IIncidentAction>();
			actions.Add(new TestFactionIncidentAction());

			var debugIncident = new Incident(typeof(FactionContext), criteria, actions);
			incidents.Add(debugIncident);
		}
	}
}