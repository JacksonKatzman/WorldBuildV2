using Game.Data;
using Game.Debug;
using Game.GUI.Adventures;
using Game.GUI.Popups;
using Game.Incidents;
using Game.Terrain;
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
		private Popup currentPopup;

		public AdventureService()
		{
			Setup();
		}

		public void FirstTimeSetup()
        {
			Shader.DisableKeyword("HEX_MAP_EDIT_MODE");
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


			world.HexGrid.AddUnit(
					GameObject.Instantiate(HexUnit.unitPrefab), CurrentLocation.GetHexCell(), Random.Range(0f, 360f)
				);

			HexMapCamera.PanToCell(CurrentLocation.GetHexCell());
		}

		public void BeginAdventure()
		{
			if(CurrentLocation == null)
            {
				FirstTimeSetup();
            }
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

			var levelAppropriateEncounters = GetLevelAppropriateEncounters(encountersInRange.ToList(), lowerDifficultyThreshold, upperDifficultyThreshold, true);

			while ((selectedEncounters.Count < adventureOfferingCount) && (levelAppropriateEncounters.Count > 0))
			{
				var chosen = SimRandom.RandomEntryFromList(levelAppropriateEncounters);
				selectedEncounters.Add(chosen);
				levelAppropriateEncounters.Remove(chosen);
			}

			var levelAppropriateEvergreenEncounters = GetLevelAppropriateEncounters(EvergreenEncounters, lowerDifficultyThreshold, upperDifficultyThreshold, true);
			while (selectedEncounters.Count < adventureOfferingCount && (levelAppropriateEvergreenEncounters.Count > 0))
			{
				var selected = SimRandom.RandomEntryFromList(levelAppropriateEvergreenEncounters);
				selectedEncounters.Add(selected);
				levelAppropriateEvergreenEncounters.Remove(selected);
			}

			//temp use of popup, change to dedicated type later
			var popupConfig = new MultiButtonPopupConfig
			{
				Description = "Choose your adventure!",
				ButtonActions = new Dictionary<string, System.Action>(),
				CloseOnButtonPress = true
			};
			foreach(var encounter in selectedEncounters)
            {
				popupConfig.ButtonActions.Add(encounter.encounterTitle, () =>
				{
					//PopupService.Instance.ClosePopup(currentPopup);
					RunAdventure(encounter, 2);
				});
            }
			currentPopup = PopupService.Instance.ShowPopup(popupConfig);
		}

		public void RunAdventure(AdventureEncounterObject encounter, int minorEncounters)
        {
			OutputLogger.Log($"Running encounter {encounter.encounterTitle}");
			RunEncounter(encounter, 2);
		}

		public void RunEncounter(AdventureEncounterObject finalEncounter, int remainingMinorEncounters)
        {
			//choose first sub adventure and show options popup
			var encounter = finalEncounter;
			if (remainingMinorEncounters > 0)
			{
				var encounters = GetNearbyLevelAppropriateEncounters(0, 3, false, CurrentLocation.GetHexCell(), 2);
				encounters.AddRange(GetLevelAppropriateEncounters(EvergreenEncounters, 0, 3, false));
				encounter = SimRandom.RandomEntryFromList(encounters);
			}

			var popupConfig = new MultiButtonPopupConfig
			{
				Title = $"{encounter.encounterTitle}",
				Description = encounter.encounterBlurb,
				ButtonActions = new Dictionary<string, System.Action>(),
				CloseOnButtonPress = true
			};

			if (remainingMinorEncounters > 0)
			{
				/*
				popupConfig.ButtonActions.Add("Complete!", () => { RunEncounter(finalEncounter, remainingMinorEncounters - 1); });
				popupConfig.ButtonActions.Add("Failure", () => { UserInterfaceService.Instance.OnEndAdventureButton(); });
				popupConfig.ButtonActions.Add("Return Home", () => { UserInterfaceService.Instance.OnEndAdventureButton(); });
				popupConfig.ButtonActions.Add("Skip", () => { RunEncounter(finalEncounter, remainingMinorEncounters); });
				*/
				popupConfig.ButtonActions.Add("Begin Encounter", () => 
				{ 
					AdventureGuide.Instance.RunEncounter(new Encounter(encounter, null), 
						() => RunEncounter(finalEncounter, remainingMinorEncounters - 1),
						() => RunEncounter(finalEncounter, remainingMinorEncounters)); 
				});
				if(encounter.skippable)
                {
					popupConfig.ButtonActions.Add("Skip", () => { RunEncounter(finalEncounter, remainingMinorEncounters); });
				}
				popupConfig.ButtonActions.Add("Return Home", () => { OnEndAdventure(encounter, false); });
			}
			else
			{
				/*
				popupConfig.ButtonActions.Add("Complete!", () => 
				{ 
					HandleRewards(encounter);
					UserInterfaceService.Instance.OnEndAdventureButton(); 
				});
				popupConfig.ButtonActions.Add("Failure", () => { UserInterfaceService.Instance.OnEndAdventureButton(); });
				*/
				popupConfig.ButtonActions.Add("Begin Encounter", () =>
				{
					AdventureGuide.Instance.RunEncounter(new Encounter(encounter, null),
						() => OnEndAdventure(encounter, true),
						() => OnEndAdventure(encounter, false));
				});
				if (encounter.skippable)
				{
					popupConfig.ButtonActions.Add("Skip", () => { RunEncounter(finalEncounter, remainingMinorEncounters); });
				}
				popupConfig.ButtonActions.Add("Return Home", () => { OnEndAdventure(encounter, false); });
			}

			currentPopup = PopupService.Instance.ShowPopup(popupConfig);
		}

		public void OnEndAdventure(AdventureEncounterObject encounter, bool success)
        {
			if(success)
            {
				HandleRewards(encounter);
            }
			UserInterfaceService.Instance.OnEndAdventureButton();
		}

		public List<AdventureEncounterObject> GetLevelAppropriateEncounters(List<AdventureEncounterObject> possibleAdventures, int lowerDifficultyThreshold, int upperDifficultyThreshold, bool major)
		{
			return possibleAdventures.Where(encounter => encounter.majorEncounter == major && encounter.encounterDifficulty >= lowerDifficultyThreshold && encounter.encounterDifficulty <= upperDifficultyThreshold).ToList();
		}

		public List<AdventureEncounterObject> GetNearbyLevelAppropriateEncounters(int lowerDifficultyThreshold, int upperDifficultyThreshold, bool major, HexCell cell, int range)
		{
			var loc = new Location(cell.Index);
			var cellsInRange = loc.GetAllCellsInRange(range);
			var encountersInRange = AvailableEncounters.Where(encounter => cellsInRange.Contains(encounter.CurrentLocation.GetHexCell()));
			return GetLevelAppropriateEncounters(encountersInRange.ToList(), lowerDifficultyThreshold, upperDifficultyThreshold, major);
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

		private void HandleRewards(AdventureEncounterObject encounterObject)
        {
			OutputLogger.Log($"Rewarding players for completing {encounterObject.encounterTitle}!");
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
