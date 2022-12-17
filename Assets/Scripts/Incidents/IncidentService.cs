using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using System;
using Game.IO;
using Game.Simulation;

namespace Game.Incidents
{
	public class IncidentService
	{
		private static IncidentService instance;

		public Dictionary<string, ExpressionValue> currentExpressionValues;

		private List<IIncident> incidents;
		private List<DelayedIncidentContext> delayedContexts;
		private int nextIncidentID;
		public List<IncidentReport> reports;

		public IIncident CurrentIncident { get; set; }

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

		public void PerformIncidents(IIncidentContext context)
		{
			var contextType = context.GetType();
			var incidentsOfType = GetIncidentsOfType(contextType);
			if(incidentsOfType == null || incidentsOfType.Count == 0)
			{
				OutputLogger.Log("No incidents of that type!");
				return;
			}

			var incidentContext = context;

			var possibleIncidents = GetIncidentsWithMatchingCriteria(incidentsOfType, incidentContext);

			if(possibleIncidents == null || possibleIncidents.Count == 0)
			{
				OutputLogger.Log("No matching incidents!");
				return;
			}

			for (int a = 0; a < context.NumIncidents; a++)
			{
				var report = new IncidentReport(nextIncidentID, context.ParentID, SimulationManager.Instance.world.Age);

				var completed = false;
				for (int i = 0; i < 5 && i < possibleIncidents.Count && !completed; i++)
				{
					CurrentIncident = SimRandom.RandomEntryFromWeightedDictionary(possibleIncidents);
					OutputLogger.Log("Attempting to run incident: " + CurrentIncident.IncidentName);
					completed = CurrentIncident.PerformIncident(incidentContext, ref report);
					currentExpressionValues.Clear();
				}

				if (completed)
				{
					nextIncidentID++;
					reports.Add(report);
				}
			}
		}

		public void PerformDelayedContexts()
		{
			foreach(var context in delayedContexts)
			{
				if(--context.delayCounter <= 0)
				{
					PerformIncidents(context.incidentContext);
				}
			}

			delayedContexts = delayedContexts.Where(x => x.delayCounter > 0).ToList();
		}

		public void AddDelayedContext(IIncidentContext context, int delay)
		{
			delayedContexts.Add(new DelayedIncidentContext(context, delay));
		}

		public void WriteIncidentLogToDisk()
		{
			var path = Path.Combine(Application.dataPath + SaveUtilities.RESOURCES_DATA_PATH + "incidentLog.txt");
			string output = string.Empty;

			foreach(var entry in reports)
			{
				output += string.Format("{0}: {1}\n", entry.ReportYear, entry.ReportLog);
			}

			File.WriteAllText(path, output);

			OutputLogger.Log("Incident Saved!");
		}

		private List<IIncident> GetIncidentsOfType(Type type)
		{
			var items = incidents.Where(x => x.ContextType == type).ToList();
			return items;
		}
		private Dictionary<int, List<IIncident>> GetIncidentsWithMatchingCriteria(List<IIncident> incidents, IIncidentContext context)
		{
			var items = incidents.Where(x => x.Criteria.Evaluate(context) == true).ToList();
			Dictionary<int, List<IIncident>> sortedItems = new Dictionary<int, List<IIncident>>();
			foreach(var item in items)
			{
				var weight = item.Weights.CalculateWeight(context);
				if(!sortedItems.ContainsKey(weight))
				{
					sortedItems.Add(weight, new List<IIncident>());
				}
				sortedItems[weight].Add(item);
			}
			return sortedItems;
		}

		private void Setup()
		{
			incidents = new List<IIncident>();
			var files = Directory.GetFiles(Application.dataPath + SaveUtilities.INCIDENT_DATA_PATH, "*.json");
			foreach (string file in files)
			{
				var text = File.ReadAllText(file);
				if (text.Length > 0)
				{
					var item = JsonConvert.DeserializeObject<Incident>(text, SaveUtilities.SERIALIZER_SETTINGS);
					incidents.Add(item);
				}
			}

			nextIncidentID = 0;
			delayedContexts = new List<DelayedIncidentContext>();
			reports = new List<IncidentReport>();

			currentExpressionValues = new Dictionary<string, ExpressionValue>();
		}
	}
}