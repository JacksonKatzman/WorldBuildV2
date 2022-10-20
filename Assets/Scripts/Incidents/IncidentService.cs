using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using UnityEngine;
using System;
using Game.IO;

namespace Game.Incidents
{
	public class IncidentService
	{
		private static IncidentService instance;

		private List<IIncident> incidents;
		private List<DelayedIncidentContext> delayedContexts;
		private int nextIncidentID;
		private List<IncidentReport> reports;

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
			PerformIncidents(incidentContextProvider.GetContext());
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

			//instead of having this grab first or default and possibly fail, have it try a few of the possible
			//incidents to see if it can complete one
			var report = new IncidentReport();
			var completed = possibleIncidents.FirstOrDefault().PerformIncident(incidentContext, ref report);

			if(completed)
			{
				nextIncidentID++;
				reports.Add(report);
			}
		}

		public void PerformDelatedContexts()
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
		}
	}

	public class IncidentReport
	{
		public int IncidentID { get; set; }
		public int ParentID { get; set; }
		public Dictionary<string, IIncidentContextProvider> Providers { get; set; }

		public string ReportLog { get; set; }

		public IncidentReport() { }
		public IncidentReport(int incidentID, int parentID)
		{
			IncidentID = incidentID;
			ParentID = parentID;
		}
	}

	public class DelayedIncidentContext
	{
		public IIncidentContext incidentContext;
		public int delayCounter;

		public DelayedIncidentContext(IIncidentContext context, int delay)
		{
			incidentContext = context;
			delayCounter = delay;
		}
	}
}