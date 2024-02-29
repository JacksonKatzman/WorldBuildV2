using Game.Incidents;
using System.Collections.Generic;
using System.Linq;

namespace Game.Simulation
{
	public class Encounter
	{
		public List<AdventureEncounterObject> Encounters => GetEncounterList();
		public List<IIncidentContext> Contexts => GetContexts();
		public List<IAdventureContextCriteria> ContextCriteria => GetContextCriteria();

		public AdventureEncounterObject mainEncounter;
		private List<AdventureEncounterObject> sideEncounters;
		public Encounter() { }
		public Encounter(AdventureEncounterObject mainEncounter, List<AdventureEncounterObject> sideEncounters)
		{
			this.mainEncounter = mainEncounter;
			this.sideEncounters = sideEncounters;
		}

		public bool TryGetContext(int id, out IIncidentContext result)
		{
			result = Contexts.Find(x => x.ID == id);
			return result != null;
			//return !result.Equals(default(IIncidentContext));
		}

		public bool TryGetContextCriteria(int id, out IAdventureContextCriteria result)
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

		private List<IAdventureContextCriteria> GetContextCriteria()
		{
			var criteria = new List<IAdventureContextCriteria>();
			foreach(var encounter in Encounters)
			{
				criteria.AddRange(encounter.contextCriterium);
			}
			return criteria;
		}
	}
}
