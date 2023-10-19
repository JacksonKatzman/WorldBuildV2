using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using System;
using Game.Simulation;
using Game.Utilities;
using Game.Debug;

namespace Game.Incidents
{
	public class IncidentService
	{
		private static IncidentService instance;

		private List<IIncident> incidents;
		private List<DelayedIncidentContext> delayedContexts;
		private List<IIncidentContext> followUpContexts;
		private int nextIncidentID;

		public List<IncidentReport> reports;
		[JsonIgnore]
		public List<IncidentReport> staticReports;

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
					OutputLogger.Log("Attempted to run incident: " + CurrentIncident.IncidentName + " Success: " + completed);
					ContextDictionaryProvider.CurrentExpressionValues.Clear();
				}

				if (completed)
				{
					nextIncidentID++;
					report.CreateFullLog();
					reports.Add(report);

					while(followUpContexts.Count > 0)
					{
						var deployedContext = followUpContexts.First();
						followUpContexts.Remove(deployedContext);
						PerformIncidents(deployedContext);
					}
				}

				followUpContexts.Clear();
			}

			reports.AddRange(staticReports);
			staticReports.Clear();
		}

		public void ReportStaticIncident(string log, List<IIncidentContext> contexts, bool isMajorIncident)
		{
			var report = new StaticIncidentReport(nextIncidentID, -1, SimulationManager.Instance.world.Age);
			report.AddLog(log);
			report.AddContexts(contexts);
			nextIncidentID++;
			report.CreateFullLog();
			report.IsMajorIncident = isMajorIncident;
			staticReports.Add(report);
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

		public void AddFollowUpContext(IIncidentContext context)
		{
			followUpContexts.Add(context);
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

			OutputLogger.Log("Incident Log saved to disk!");
		}

		public void SaveIncidentLog(string mapName)
		{
			ES3.Save("nextIncidentID", nextIncidentID, SaveUtilities.GetIncidentLogPath(mapName));
			ES3.Save("reports", reports, SaveUtilities.GetIncidentLogPath(mapName));
			//In future we should save and load the delayedContexts
			//to allow GMs to set things in motion and have those be remembered.
		}

		public void LoadIncidentLog(string mapName)
		{
			nextIncidentID = ES3.Load<int>("nextIncidentID", SaveUtilities.GetIncidentLogPath(mapName));
			reports = ES3.Load<List<IncidentReport>>("reports", SaveUtilities.GetIncidentLogPath(mapName));
		}

		private List<IIncident> GetIncidentsOfType(Type type)
		{
			var items = incidents.Where(x => x.ContextType == type).ToList();
			return items;
		}
		private Dictionary<int, List<IIncident>> GetIncidentsWithMatchingCriteria(List<IIncident> incidents, IIncidentContext context)
		{
			var items = incidents.Where(x => x.Criteria.Evaluate(context) == true && x.WorldCriteria.Evaluate(context) == true).ToList();
			Dictionary<int, List<IIncident>> sortedItems = new Dictionary<int, List<IIncident>>();
			foreach(var item in items)
			{
				var weight = item.Weights.CalculateWeight(context);
				if(weight <= 0)
				{
					continue;
				}
				if(!sortedItems.ContainsKey(weight))
				{
					sortedItems.Add(weight, new List<IIncident>());
				}
				sortedItems[weight].Add(item);
			}
			return sortedItems;
		}

		public void Setup()
		{
			nextIncidentID = 0;
			delayedContexts = new List<DelayedIncidentContext>();
			followUpContexts = new List<IIncidentContext>();
			reports = new List<IncidentReport>();
			staticReports = new List<IncidentReport>();

			ContextDictionaryProvider.CurrentExpressionValues = new Dictionary<string, ExpressionValue>();
		}

		public void CompileIncidents()
		{
			incidents = new List<IIncident>();

			foreach(var asset in AssetService.Instance.incidents.texts)
			{
				if (asset.text.Length > 0)
				{
					var item = JsonConvert.DeserializeObject<Incident>(asset.text, SaveUtilities.SERIALIZER_SETTINGS);
					incidents.Add(item);
				}
			}
		}
	}
}