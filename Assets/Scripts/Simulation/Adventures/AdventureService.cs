using Game.Data;
using Game.Debug;
using Game.GUI.Popups;
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

		//pick city
		//move camera to city
		//spawn unit
		//exp X time matrix
		//load possible adventures for the area
		//suggest 3
		//choose 1
		//walk to next tile
		//get possible subadventure for new tile
		//run it(popup with the 4 options for resolution)
		//repeat
		//reward step

		public List<AdventureEncounterObject> EvergreenEncounters { get; set; }
		public List<AdventureEncounterObject> AvailableEncounters { get; set; }
		public List<AdventureEncounterObject> UsedEncounters { get; set; }

		public List<MonsterData> monsterData;

		public Location CurrentLocation { get; set; }

		private World world = World.CurrentWorld;

		public AdventureService()
		{
			Setup();
		}

		public void BeginAdventure()
		{
			//Pick location for players to start, likely in one of the towns/hamlets
			var startingCity = SimRandom.RandomEntryFromList(world.Cities);
			CurrentLocation = startingCity.CurrentLocation;
			//Generate layout of town/what its contents is
			//Generate all the points of interest/people of interest in the town
			//Generate NPCs for the tavern the players start in
			startingCity.GenerateMinorCharacters(5);
			//Generate adventure based in location/people of interest etc
			//Extra credit: generate world points of interest in case players want to explore for their adventures instead?
			//That or just include exploration contracts among the possible adventures
			if (CurrentLocation == null)
			{
				OutputLogger.LogError("Current Location in AdventureService is null!");
				return;
			}

			var range = 4; //more complex calc later based on party level etc
			var cellsInRange = CurrentLocation.GetAllCellsInRange(range);
			var encountersInRange = AvailableEncounters.Where(encounter => cellsInRange.Contains(encounter.CurrentLocation.GetHexCell()));

			var adventureOfferingCount = 3;
			var lowerDifficultyThreshold = 0;
			var upperDifficultyThreshold = 3;
			var selectedEncounters = new List<AdventureEncounterObject>();

			var levelAppropriateEncounters = GetLevelAppropriateEncounters(encountersInRange.ToList(), lowerDifficultyThreshold, upperDifficultyThreshold);

			while ((selectedEncounters.Count < adventureOfferingCount) && (levelAppropriateEncounters.Count > 0))
			{
				var chosen = SimRandom.RandomEntryFromList(levelAppropriateEncounters);
				selectedEncounters.Add(chosen);
				levelAppropriateEncounters.Remove(chosen);
			}

			var levelAppropriateEvergreenEncounters = GetLevelAppropriateEncounters(EvergreenEncounters, lowerDifficultyThreshold, upperDifficultyThreshold);
			while (selectedEncounters.Count < adventureOfferingCount && (levelAppropriateEvergreenEncounters.Count > 0))
			{
				var selected = SimRandom.RandomEntryFromList(levelAppropriateEvergreenEncounters);
				selectedEncounters.Add(selected);
				levelAppropriateEvergreenEncounters.Remove(selected);
			}

			//temp use of popup, change to dedicated type later
			var popupConfig = new MultiButtonPopupConfig
			{
				ButtonActions = new Dictionary<string, System.Action>()
			};
			foreach(var encounter in selectedEncounters)
            {
				popupConfig.ButtonActions.Add(encounter.encounterTitle, () => RunEncounter(encounter));
            }
			PopupService.Instance.ShowPopup(popupConfig);
		}

		public void RunEncounter(AdventureEncounterObject encounter)
        {
			OutputLogger.Log($"Running encounter {encounter.encounterTitle}");
        }

		public List<AdventureEncounterObject> GetLevelAppropriateEncounters(List<AdventureEncounterObject> possibleAdventures, int lowerDifficultyThreshold, int upperDifficultyThreshold)
		{
			return possibleAdventures.Where(encounter => encounter.majorEncounter && encounter.encounterDifficulty >= lowerDifficultyThreshold && encounter.encounterDifficulty <= upperDifficultyThreshold).ToList();
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

			if (AssetService.Instance.objectData.collections.ContainsKey(typeof(AdventureEncounterObject)))
			{
				var values = AssetService.Instance.objectData.collections[typeof(AdventureEncounterObject)].objects.Values.ToList();
				foreach(var obj in values)
				{
					EvergreenEncounters.Add(obj as AdventureEncounterObject);
				}
			}
			OutputLogger.Log(string.Format("{0} encounters loaded.", EvergreenEncounters.Count));

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
