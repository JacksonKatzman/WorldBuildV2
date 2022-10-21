using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Game.WorldGeneration;
using Game.Enums;

namespace Game.Incidents
{
	public class OldIncidentService : ITimeSensitive
	{
		private static OldIncidentService instance;
		public static OldIncidentService Instance
		{
			get
			{
				if(instance == null)
				{
					instance = new OldIncidentService();
				}
				return instance;
			}
		}

		private List<CoreIncident> coreIncidents;
		private List<CoreIncident> recordedIncidents;
		private List<OldDelayedIncidentContext> contextQueue;

		private OldIncidentService()
		{
			//compile all core incidents into data structure for later use
			coreIncidents = new List<CoreIncident>();
			recordedIncidents = new List<CoreIncident>();
			contextQueue = new List<OldDelayedIncidentContext>();

			//coreIncidents.AddRange(IncidentEditorWindow.ParseIncidents());

			//coreIncidents.Add(new CoreIncident(new List<IncidentTag>(), 1, new NullIncidentModifier(), new List<IncidentModifier> { new NullIncidentModifier() }, new List<IncidentModifier> { new NullIncidentModifier() }));

			//coreIncidents.Add(new CoreIncident(new List<IIncidentTag> { new InstigatorTag(typeof(World)), new WorldTag(new List<WorldTagType> { WorldTagType.DEATH})},
				//100, new List<IncidentModifier> { new DeathModifier(), new OldAgeModifier() }, new List<IncidentModifier> { new NullIncidentModifier() }));

		}

		public void PerformIncident(OldIncidentContext incidentContext)
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

		public void QueueDelayedIncident(OldIncidentContext context, int timer)
		{
			var delayedContext = new OldDelayedIncidentContext(context, timer);
			contextQueue.Add(delayedContext);
		}

		public void AdvanceTime()
		{
			var finishedContexts = new List<OldDelayedIncidentContext>();

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

	public class OldDelayedIncidentContext
	{
		private int timer;
		private OldIncidentContext context;

		public OldDelayedIncidentContext(OldIncidentContext context, int timer)
		{
			this.context = context;
			this.timer = timer;
		}

		public bool Advance()
		{
			timer--;
			if(timer <= 0)
			{
				OldIncidentService.Instance.PerformIncident(context);
				return true;
			}
			return false;
		}
	}
}