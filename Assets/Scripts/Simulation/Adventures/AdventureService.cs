using Game.Data;
using Game.Debug;
using Game.Enums;
using Game.GUI.Adventures;
using Game.GUI.Popups;
using Game.Incidents;
using Game.Terrain;
using Game.Utilities;
using System;
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
		public Location AdventureStartLocation { get; set; }

		private bool isDungeonMasterView;
		public bool IsDungeonMasterView
        {
			get
            {
				return isDungeonMasterView;
            }
			set
            {
				isDungeonMasterView = value;
				World.CurrentWorld.HexGrid.SetMapVisibility(isDungeonMasterView);
				EventManager.Instance.Dispatch(new IsDungeonMasterViewChangedEvent { isDungeonMasterView = isDungeonMasterView });
            }
        }

		public Dictionary<Type, Dictionary<IIncidentContext, ContextFamiliarity>> KnownContexts { get; set; }

		private HexUnit partyUnit;
		public HexUnit PartyUnit
        {
            get
            {
				return partyUnit;
            }
			set
            {
				if(value != partyUnit)
                {
					if (partyUnit != null)
					{
						GameObject.Destroy(partyUnit.gameObject);
					}
					partyUnit = value;
                }
            }
        }

		//private List<HexCell> AdventurePath { get; set; }
		private Queue<HexCell> AdventureDestinations { get; set; }

		private Adventure currentAdventure;
		private Popup currentPopup;

		public AdventureService()
		{
			Setup();
		}

		public void FirstTimeSetup()
        {
			//Pick location for players to start, likely in one of the towns/hamlets
			var startingCity = SimRandom.RandomEntryFromList(World.CurrentWorld.Cities);
			CurrentLocation = startingCity.CurrentLocation;
			//Generate layout of town/what its contents is
			//Generate all the points of interest/people of interest in the town
			//Generate NPCs for the tavern the players start in
			startingCity.GenerateMinorCharacters(5);
			//Generate adventure based in location/people of interest etc
			//Extra credit: generate world points of interest in case players want to explore for their adventures instead?
			//That or just include exploration contracts among the possible adventures

			PartyUnit = GameObject.Instantiate(HexUnit.unitPrefab);
			World.CurrentWorld.HexGrid.AddUnit(
					PartyUnit, CurrentLocation.GetHexCell(), 0.0f
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

			AdventureStartLocation = CurrentLocation;
			var range = 4; //more complex calc later based on party level etc
			var numSubEncounters = 2; //more complex calc later based on party level etc
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
				if (DetermineEncounterLocation(encounter, cellsInRange, numSubEncounters))
				{
					popupConfig.ButtonActions.Add(encounter.encounterTitle, () =>
					{
					//PopupService.Instance.ClosePopup(currentPopup);
					RunAdventure(encounter, numSubEncounters);
					});
				}
            }
			currentPopup = PopupService.Instance.ShowPopup(popupConfig);
		}

		public void RunAdventure(AdventureEncounterObject encounter, int minorEncounters)
        {
			currentAdventure = new Adventure(encounter, new List<AdventureEncounterObject>());

			var grid = World.CurrentWorld.HexGrid;
			grid.ClearPath();
			grid.FindPathWithUnit(CurrentLocation.GetHexCell(), encounter.CurrentLocation.GetHexCell(), PartyUnit);
			var path = grid.GetPath();

			AdventureDestinations.Clear();
			for(int i = 0; i < minorEncounters; i++)
            {
				AdventureDestinations.Enqueue(path[(i * (path.Count / minorEncounters)) + 1]);
            }
			AdventureDestinations.Enqueue(path[path.Count - 1]);

			OutputLogger.Log($"Running encounter {encounter.encounterTitle}");
			MoveToNextEncounter(encounter, minorEncounters);
		}

		public void MoveToNextEncounter(AdventureEncounterObject finalEncounter, int remainingMinorEncounters)
        {
			//move to encounter location
			var destination = AdventureDestinations.Dequeue();
			var grid = World.CurrentWorld.HexGrid;
			grid.ClearPath();
			grid.FindPathWithUnit(CurrentLocation.GetHexCell(), destination, PartyUnit);
			var path = grid.GetPath();

			if (path.Count > 1)
			{
				CurrentLocation = new Location(destination.Index);
				PartyUnit.Travel(path, () => { RunEncounter(finalEncounter, remainingMinorEncounters); });
			}
			else
            {
				RunEncounter(finalEncounter, remainingMinorEncounters);
			}
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
				popupConfig.ButtonActions.Add("Begin Encounter", () => 
				{ 
					AdventureGuide.Instance.RunEncounter(encounter, 
						() => { MoveToNextEncounter(finalEncounter, remainingMinorEncounters - 1); currentAdventure.AddEncounter(encounter); },
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
				popupConfig.ButtonActions.Add("Begin Encounter", () =>
				{
					AdventureGuide.Instance.RunEncounter(encounter,
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
			//KINDA REALLY just wanna rework this entire flow to stick everything into the adventure as it goes and run it that way

			//add pathfinding home
			if(success)
            {
				HandleRewards(encounter);
            }

			var grid = World.CurrentWorld.HexGrid;
			grid.ClearPath();
			grid.FindPathWithUnit(CurrentLocation.GetHexCell(), AdventureStartLocation.GetHexCell(), PartyUnit);
			var path = grid.GetPath();

			CurrentLocation = AdventureStartLocation;
			PartyUnit.Travel(path, () => { UserInterfaceService.Instance.OnEndAdventureButton(); });

			foreach(var context in currentAdventure.ContextCriteria)
            {
				if(!ContextDictionaryProvider.AllContexts[context.ContextType].Contains(context.Context))
                {
					EventManager.Instance.Dispatch(new AddContextEvent(context.Context, context.Context.ContextType, true));
                }
				AddKnownContext(context.Context);
            }
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

		public void AddKnownContext(IIncidentContext context, ContextFamiliarity familiarity = ContextFamiliarity.AWARE)
        {
			if(KnownContexts == null)
            {
				KnownContexts = new Dictionary<Type, Dictionary<IIncidentContext, ContextFamiliarity>>();
            }

			if(!KnownContexts.ContainsKey(context.ContextType))
            {
				KnownContexts.Add(context.ContextType, new Dictionary<IIncidentContext, ContextFamiliarity>());
            }

			if (!KnownContexts[context.ContextType].ContainsKey(context))
			{
				KnownContexts[context.ContextType].Add(context, familiarity);
			}
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

		private bool DetermineEncounterLocation(AdventureEncounterObject encounterObject, List<HexCell> cellsInRange, int numSubEncounters)
        {
			if(encounterObject.CurrentLocation != null)
            {
				return true;
            }

			var cell = CurrentLocation.GetHexCell();
			//temporary - will eventually factor in that certain encounters can only happen in certain places
			var outerRange = cellsInRange.Where(x => x.coordinates.DistanceTo(cell.coordinates) > numSubEncounters && !x.IsUnderwater).ToList();

			List<HexCell> path = null;
			var grid = World.CurrentWorld.HexGrid;
			int tries = 0;
			HexCell possibleCell = null;
			while(path == null && tries < 50)
            {
				possibleCell = SimRandom.RandomEntryFromList(outerRange);
				grid.ClearPath();
				grid.FindPathWithUnit(CurrentLocation.GetHexCell(), possibleCell, PartyUnit);
				path = grid.GetPath();
				tries++;
			}

			if (possibleCell != null)
			{
				encounterObject.CurrentLocation = new Location(possibleCell.Index);
				return true;
			}
			else
            {
				return false;
            }
        }

		private void HandleRewards(AdventureEncounterObject encounterObject)
        {
			OutputLogger.Log($"Rewarding players for completing {encounterObject.encounterTitle}!");
        }

		private void UpdateWiki(AdventureEncounterObject encounterObject)
        {

        }

		private void Setup()
		{
			EvergreenEncounters = new List<AdventureEncounterObject>();
			AvailableEncounters = new List<AdventureEncounterObject>();
			UsedEncounters = new List<AdventureEncounterObject>();
			monsterData = new List<MonsterData>();
			AdventureDestinations = new Queue<HexCell>();
			KnownContexts = new Dictionary<Type, Dictionary<IIncidentContext, ContextFamiliarity>>();

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
