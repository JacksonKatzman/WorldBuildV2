using Game.Creatures;
using Game.Incidents;
using Game.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureService : ILocationAffiliated
	{
		private static AdventureService instance;
		public static AdventureService Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new AdventureService();
				}
				return instance;
			}
		}

		public List<AdventureEncounterObject> evergreenEncounters;
		public List<AdventureEncounterObject> availableEncounters;
		public List<AdventureEncounterObject> usedEncounters;

		public List<MonsterData> monsterData;

		public Location CurrentLocation { get; set; }

		public AdventureService()
		{
			Setup();
		}

		public void SetAdventureStartingPoint(Location location)
		{
			CurrentLocation = location;
		}

		public void BeginAdventure()
		{
			if(CurrentLocation == null)
			{
				OutputLogger.LogError("Current Location in AdventureService is null!");
				return;
			}

			var range = 4; //more complex calc later based on party level etc
			var cellsInRange = CurrentLocation.GetAllCellsInRange(range);
			var encountersInRange = availableEncounters.Where(encounter => cellsInRange.Contains(encounter.CurrentLocation.GetHexCell()));

			var adventureOfferingCount = 5; 
			var lowerDifficultyThreshold = 0;
			var upperDifficultyThreshold = 3;
			var selectedEncounters = new List<AdventureEncounterObject>();

			var levelAppropriateEncounters = GetLevelAppropriateEncounters(encountersInRange.ToList(), lowerDifficultyThreshold, upperDifficultyThreshold);
			
			while((selectedEncounters.Count < adventureOfferingCount) && (levelAppropriateEncounters.Count > 0))
			{
				var chosen = SimRandom.RandomEntryFromList(levelAppropriateEncounters);
				selectedEncounters.Add(chosen);
				levelAppropriateEncounters.Remove(chosen);
			}

			var levelAppropriateEvergreenEncounters = GetLevelAppropriateEncounters(evergreenEncounters, lowerDifficultyThreshold, upperDifficultyThreshold);
			while(selectedEncounters.Count < adventureOfferingCount)
			{
				selectedEncounters.Add(SimRandom.RandomEntryFromList(levelAppropriateEvergreenEncounters));
			}
		}

		public List<AdventureEncounterObject> GetLevelAppropriateEncounters(List<AdventureEncounterObject> possibleAdventures, int lowerDifficultyThreshold, int upperDifficultyThreshold)
		{
			return possibleAdventures.Where(encounter => encounter.encounterDifficulty >= lowerDifficultyThreshold && encounter.encounterDifficulty <= upperDifficultyThreshold).ToList();
		}

		public void AddAvailableEncounter(AdventureEncounterObject encounter)
		{
			availableEncounters.Add(encounter);
		}

		private void Setup()
		{
			evergreenEncounters = new List<AdventureEncounterObject>();
			availableEncounters = new List<AdventureEncounterObject>();
			usedEncounters = new List<AdventureEncounterObject>();
			monsterData = new List<MonsterData>();

			var encountersPath = "ScriptableObjects/Encounters";

			evergreenEncounters.AddRange(Resources.LoadAll(encountersPath, typeof(AdventureEncounterObject)).Cast<AdventureEncounterObject>().ToList());
			OutputLogger.Log(string.Format("{0} encounters loaded.", evergreenEncounters.Count));

			var monstersPath = "ScriptableObjects/Monsters";
			monsterData.AddRange(Resources.LoadAll(monstersPath, typeof(MonsterData)).Cast<MonsterData>().ToList());
			OutputLogger.Log(string.Format("{0} monsters loaded.", monsterData.Count));
		}
	}
}
