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

		public List<AdventureEncounterObject> EvergreenEncounters { get; set; }
		public List<AdventureEncounterObject> AvailableEncounters { get; set; }
		public List<AdventureEncounterObject> UsedEncounters { get; set; }

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
			var encountersInRange = AvailableEncounters.Where(encounter => cellsInRange.Contains(encounter.CurrentLocation.GetHexCell()));

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

			var levelAppropriateEvergreenEncounters = GetLevelAppropriateEncounters(EvergreenEncounters, lowerDifficultyThreshold, upperDifficultyThreshold);
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
			AvailableEncounters.Add(encounter);
		}

		public void Save(string mapName)
		{
			ES3.Save("AvailableEncounters", AvailableEncounters, SaveUtilities.GetAdventureSavePath(mapName));
			ES3.Save("UsedEncounters", UsedEncounters, SaveUtilities.GetAdventureSavePath(mapName));
		}

		public void Load(string mapName)
		{
			AvailableEncounters = ES3.Load<List<AdventureEncounterObject>>("AvailableEncounters", SaveUtilities.GetAdventureSavePath(mapName));
			UsedEncounters = ES3.Load<List<AdventureEncounterObject>>("UsedEncounters", SaveUtilities.GetAdventureSavePath(mapName));
		}

		private void Setup()
		{
			EvergreenEncounters = new List<AdventureEncounterObject>();
			AvailableEncounters = new List<AdventureEncounterObject>();
			UsedEncounters = new List<AdventureEncounterObject>();
			monsterData = new List<MonsterData>();

			//var encountersPath = "ScriptableObjects/Encounters";

			//EvergreenEncounters.AddRange(Resources.LoadAll(encountersPath, typeof(AdventureEncounterObject)).Cast<AdventureEncounterObject>().ToList());
			if (AssetService.Instance.objectData.collections.ContainsKey(typeof(AdventureEncounterObject)))
			{
				var values = AssetService.Instance.objectData.collections[typeof(AdventureEncounterObject)].objects.Values.ToList();
				foreach(var obj in values)
				{
					EvergreenEncounters.Add(obj as AdventureEncounterObject);
				}
			}
			OutputLogger.Log(string.Format("{0} encounters loaded.", EvergreenEncounters.Count));

			//var monstersPath = "ScriptableObjects/Monsters";
			//monsterData.AddRange(Resources.LoadAll(monstersPath, typeof(MonsterData)).Cast<MonsterData>().ToList());
			if (AssetService.Instance.objectData.collections.ContainsKey(typeof(MonsterData)))
			{
				var values = AssetService.Instance.objectData.collections[typeof(MonsterData)].objects.Values.ToList();
				foreach(var obj in values)
				{
					monsterData.Add(obj as MonsterData);
				}
			}
			OutputLogger.Log(string.Format("{0} monsters loaded.", monsterData.Count));
		}
	}
}
