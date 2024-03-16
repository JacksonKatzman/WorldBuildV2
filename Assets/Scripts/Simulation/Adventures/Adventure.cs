using Game.Incidents;
using System.Collections.Generic;
using System.Linq;

namespace Game.Simulation
{
	public class Adventure
	{
		public List<AdventureEncounterObject> Encounters => GetEncounterList();
		public List<IIncidentContext> Contexts => GetContexts();
		public List<IAdventureContextRetriever> ContextCriteria => GetContextCriteria();
		public int RemainingSideEncounters => numSideEncounters - sideEncounters.Count;

		public AdventureEncounterObject mainEncounter;
		public int numSideEncounters;
		private List<AdventureEncounterObject> sideEncounters;
		public Adventure() { }
		public Adventure(AdventureEncounterObject mainEncounter, List<AdventureEncounterObject> sideEncounters, int numSideEncounters)
		{
			this.mainEncounter = mainEncounter;
			this.sideEncounters = sideEncounters;
			this.numSideEncounters = numSideEncounters;
		}

		public void AddEncounter(AdventureEncounterObject encounter)
        {
			if (sideEncounters.Count < this.numSideEncounters)
			{
				sideEncounters.Add(encounter);
			}
        }

		public bool TryGetContext(int id, out IIncidentContext result)
		{
			result = Contexts.Find(x => x.ID == id);
			return result != null;
			//return !result.Equals(default(IIncidentContext));
		}

		public bool TryGetContextCriteria(int id, out IAdventureContextRetriever result)
		{
			result = ContextCriteria.Find(x => x.Context.ID == id);
			return result != null;
		}

		private List<AdventureEncounterObject> GetEncounterList()
		{
			var list = new List<AdventureEncounterObject>();
			if (sideEncounters != null)
			{
				list.AddRange(sideEncounters);
			}
			list.Add(mainEncounter);
			return list;
		}

		private List<IIncidentContext> GetContexts()
		{
			var contexts = new List<IIncidentContext>();
			foreach(var encounter in Encounters)
			{
				contexts.AddRange(encounter.contextCriterium.Select(x => x.Context));
			}

			return contexts;
		}

		private List<IAdventureContextRetriever> GetContextCriteria()
		{
			var criteria = new List<IAdventureContextRetriever>();
			foreach(var encounter in Encounters)
			{
				criteria.AddRange(encounter.contextCriterium);
			}
			return criteria;
		}
	}
}
