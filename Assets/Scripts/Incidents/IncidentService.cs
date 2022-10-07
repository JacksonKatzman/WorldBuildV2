using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.WorldGeneration;
using Game.Enums;

namespace Game.Incidents
{
	public class IncidentService : ITimeSensitive
	{
		private static IncidentService instance;
		public static IncidentService Instance
		{
			get
			{
				if(instance == null)
				{
					instance = new IncidentService();
				}
				return instance;
			}
		}

		private List<CoreIncident> coreIncidents;
		private List<CoreIncident> recordedIncidents;
		private List<DelayedIncidentContext> contextQueue;

		private IncidentService()
		{
			//compile all core incidents into data structure for later use
			coreIncidents = new List<CoreIncident>();
			recordedIncidents = new List<CoreIncident>();
			contextQueue = new List<DelayedIncidentContext>();

			//coreIncidents.AddRange(IncidentEditorWindow.ParseIncidents());

			//coreIncidents.Add(new CoreIncident(new List<IncidentTag>(), 1, new NullIncidentModifier(), new List<IncidentModifier> { new NullIncidentModifier() }, new List<IncidentModifier> { new NullIncidentModifier() }));

			//coreIncidents.Add(new CoreIncident(new List<IIncidentTag> { new InstigatorTag(typeof(World)), new WorldTag(new List<WorldTagType> { WorldTagType.DEATH})},
				//100, new List<IncidentModifier> { new DeathModifier(), new OldAgeModifier() }, new List<IncidentModifier> { new NullIncidentModifier() }));

		}

		public void PerformIncident(IncidentContext incidentContext)
		{
			//Get all possible Core Incidents
			var possibleIncidents = new List<CoreIncident>();
			//Prune based on Tags
			possibleIncidents.AddRange(coreIncidents.Where(x => x.MatchesCriteria(incidentContext)));
			//Roll for Final Incident
			var incidentDictionary = new Dictionary<int, List<CoreIncident>>();
			foreach(var incident in possibleIncidents)
			{
				if(!incidentDictionary.Keys.Contains(incident.weight))
				{
					incidentDictionary.Add(incident.weight, new List<CoreIncident>());
				}
				incidentDictionary[incident.weight].Add(incident);
			}
			//Spawn and Run Incident
			var chosenIncident = new CoreIncident(SimRandom.RandomEntryFromWeightedDictionary(incidentDictionary), incidentContext);
			chosenIncident.Run();

			recordedIncidents.Add(chosenIncident);
		}

		public void QueueDelayedIncident(IncidentContext context, int timer)
		{
			var delayedContext = new DelayedIncidentContext(context, timer);
			contextQueue.Add(delayedContext);
		}

		public void AdvanceTime()
		{
			var finishedContexts = new List<DelayedIncidentContext>();

			foreach(var context in contextQueue)
			{
				if(context.Advance())
				{
					finishedContexts.Add(context);
				}
			}

			finishedContexts.ForEach(x => contextQueue.Remove(x));
		}
    }

	public class DelayedIncidentContext
	{
		private int timer;
		private IncidentContext context;

		public DelayedIncidentContext(IncidentContext context, int timer)
		{
			this.context = context;
			this.timer = timer;
		}

		public bool Advance()
		{
			timer--;
			if(timer <= 0)
			{
				IncidentService.Instance.PerformIncident(context);
				return true;
			}
			return false;
		}
	}
}