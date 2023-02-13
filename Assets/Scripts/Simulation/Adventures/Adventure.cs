using System.Collections.Generic;

namespace Game.Simulation
{
	public class Adventure
	{
		public List<AdventureEncounterObject> Encounters => GetEncounterList();

		public AdventureEncounterObject mainEncounter;
		private List<AdventureEncounterObject> sideEncounters;
		public Adventure() { }
		public Adventure(AdventureEncounterObject mainEncounter, List<AdventureEncounterObject> sideEncounters)
		{
			this.mainEncounter = mainEncounter;
			this.sideEncounters = sideEncounters;
		}

		private List<AdventureEncounterObject> GetEncounterList()
		{
			var list = new List<AdventureEncounterObject>(sideEncounters);
			list.Add(mainEncounter);
			return list;
		}
	}
}
