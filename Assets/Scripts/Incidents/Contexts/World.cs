﻿using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Factions;
using Game.Generators.Items;
using Game.Incidents;
using Game.Terrain;
using Game.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace Game.Simulation
{
	public static class WorldExtensions
	{
		public static float SimulationCompletionPercentage(this World world)
		{
			return (world.Age / world.simulationOptions.simulatedYears);
		}
		public static bool ShouldIncreaseSpecialFactions(this World world)
		{
			return SimulationCompletionPercentage(world) >= (world.NumSpecialFactions / world.simulationOptions.targetSpecialFactions);
		}

		public static bool ShouldIncreaseFactions(this World world)
		{
			return SimulationCompletionPercentage(world) >= (world.NumSpecialFactions / world.simulationOptions.targetSpecialFactions);
		}

		public static bool ShouldIncreaseCharacters(this World world)
		{
			return SimulationCompletionPercentage(world) >= (world.NumPeople / world.simulationOptions.targetCharacters);
		}

		public static bool ShouldIncreaseGreatMonsters(this World world)
		{
			return SimulationCompletionPercentage(world) >= (world.NumGreatMonsters / world.simulationOptions.targetGreatMonsters);
		}

		public static bool ShouldClaimMoreTerritory(this World world)
		{
			return SimulationCompletionPercentage(world) >= (SimulationUtilities.GetClaimedCells().Count / (world.HexGrid.cells.Count() * world.simulationOptions.claimedHexPercentage));
		}
	}
	public class World : IncidentContext
	{
		public HexGrid HexGrid => SimulationManager.Instance.HexGrid;

		public IncidentContextDictionary CurrentContexts { get; private set; }
		public IncidentContextDictionary AllContexts { get; private set; }
		private IncidentContextDictionary contextsToAdd;
		private IncidentContextDictionary contextsToRemove;

		override public int ID
		{
			get
			{
				return 0;
			}
			set
			{

			}
		}

		public int Age { get; set; }

		[JsonIgnore]
		public List<Character> People => CurrentContexts[typeof(Character)].Cast<Character>().ToList();
		[JsonIgnore]
		public List<Faction> Factions => CurrentContexts[typeof(Faction)].Cast<Faction>().ToList();
		[JsonIgnore]
		public List<City> Cities => CurrentContexts[typeof(City)].Cast<City>().ToList();
		[JsonIgnore]
		public int NumPeople => People.Count;
		public bool RoomForPeople => this.ShouldIncreaseCharacters();
		public int NumFactions => Factions.Count;
		public bool RoomForFactions => this.ShouldIncreaseFactions();
		public int NumSpecialFactions => Factions.Where(x => x.IsSpecialFaction).Count();
		public bool RoomForSpecialFaction => this.ShouldIncreaseSpecialFactions();
		public int NumGreatMonsters => CurrentContexts[typeof(GreatMonster)].Cast<GreatMonster>().ToList().Count;
		public bool RoomForGreatMonsters => this.ShouldIncreaseGreatMonsters();
		public bool CanClaimMoreTerritory => this.ShouldClaimMoreTerritory();

		public List<Item> LostItems;

		public SimulationOptions simulationOptions;

		public int nextID;

		public World()
		{
			nextID = ID + 1;
			Age = 0;

			CurrentContexts = new IncidentContextDictionary();
			AllContexts = new IncidentContextDictionary();
			contextsToAdd = new IncidentContextDictionary();
			contextsToRemove = new IncidentContextDictionary();
			LostItems = new List<Item>();
		}

		public void Initialize(List<FactionPreset> factions, SimulationOptions options)
		{
			simulationOptions = options;
			CreateRacesAndFactions(factions);
		}

		public async UniTask AdvanceTime()
		{
			ContextDictionaryProvider.DelayedRemoveContexts();
			ContextDictionaryProvider.DelayedAddContexts();

			UpdateContext();
			foreach(var contextList in CurrentContexts.Values)
			{
				foreach(var context in contextList)
				{
					context.UpdateContext();
				}
			}

			DeployContext();
			foreach (var contextList in CurrentContexts.Values)
			{
				foreach (var context in contextList)
				{
					context.DeployContext();
				}
			}

			UpdateHistoricalData();
			foreach (var contextList in CurrentContexts.Values)
			{
				foreach (var context in contextList)
				{
					context.UpdateHistoricalData();
				}
			}

			await UniTask.Yield();
		}

		public void PostSimulationCleanup()
		{
			foreach (var contextList in CurrentContexts.Values)
			{
				foreach (var context in contextList)
				{
					context.CheckForDeath();
				}
			}

			ContextDictionaryProvider.DelayedRemoveContexts();
			ContextDictionaryProvider.DelayedAddContexts();
		}

		public void BeginPostGeneration()
		{
			foreach(var faction in Factions)
			{
				if (!faction.IsSpecialFaction)
				{
					//GenerateAdditionalCities(faction);
				}

				//Create villages and add farm land

				//Show borders
				DrawFactionBorders(faction);
			}

			DrawCities();

			//Pick location for players to start, likely in one of the towns/hamlets
			var startingCity = SimRandom.RandomEntryFromList(Cities);
			//Generate layout of town/what its contents is
			//Generate all the points of interest/people of interest in the town
			//Generate NPCs for the tavern the players start in
			startingCity.GenerateMinorCharacters(5);
			//Generate adventure based in location/people of interest etc
			//Extra credit: generate world points of interest in case players want to explore for their adventures instead?
			//That or just include exploration contracts among the possible adventures
			AdventureService.Instance.SetAdventureStartingPoint(startingCity.CurrentLocation);
		}

		public void GenerateAdditionalCities(Faction faction)
		{
			if(faction.IsSpecialFaction)
			{
				return;
			}

			var totalTiles = faction.ControlledTiles;
			var tilesToBeOccupied = totalTiles * (SimRandom.RandomFloat01() / 2);
			tilesToBeOccupied -= faction.NumCities;

			for (int i = 0; i < tilesToBeOccupied; i++)
			{
				var possibleTiles = SimulationUtilities.FindCitylessCellWithinFaction(faction, 2);
				if (possibleTiles.Count == 0)
				{
					break;
				}
				var ordered = possibleTiles.OrderByDescending(x => HexGrid.GetCell(x).CalculateInhabitability());
				var chosenLocationIndex = ordered.First();
				var population = SimRandom.RandomRange((int)(faction.Cities[0].Population * 0.3f), (int)(faction.Cities[0].Population * 0.7f));
				var createdCity = new City(faction, new Location(chosenLocationIndex), population, 0);
				faction.Cities.Add(createdCity);
				EventManager.Instance.Dispatch(new AddContextImmediateEvent(createdCity));
			}
		}

		public void DrawCities()
		{
			foreach(var city in Cities)
			{
				var location = city.CurrentLocation.TileIndex;
				var tile = HexGrid.GetCell(location);

				//Change the model based on the population, will use temp stuff for now
				if (city.Population >= 2000)
				{
					tile.LandmarkType = Enums.LandmarkType.TOWER;
				}
				else
				{
					tile.LandmarkType = Enums.LandmarkType.TOWER;
				}
			}
		}

		public void DrawFactionBorders(Faction faction)
		{
			//VisualTestCentroidsHullsAndBorders(faction);

			var borderCells = SimulationUtilities.FindBorderWithinFaction(faction);

			foreach (var index in borderCells)
			{
				var cell = HexGrid.GetCell(index);
				cell.hexCellLabel.SetText(faction.Name);
				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
				{
					HexCell neighbor = cell.GetNeighbor(d);
					if (!neighbor)
					{
						continue;
					}

					var controlledCells = faction.ControlledTileIndices;

					if (!controlledCells.Contains(neighbor.Index))
					{
						cell.hexCellLabel.ToggleBorder(d, true, Color.white);
					}
				}
			}
		}

		private void VisualTestCentroidsHullsAndBorders(Faction faction)
		{
			var hullPoints = SimulationUtilities.GetConvexHull(faction.ControlledTileIndices);
			foreach (var cell in hullPoints)
			{
				//cell.hexCellLabel.ToggleBorder(d, true, Color.red);
				foreach (var border in cell.hexCellLabel.hexCellBorderImages.Values)
				{
					if (border.enabled)
					{
						border.color = Color.red;
					}
				}
			}

			faction.ClaimTerritoryBetweenCities();
		}

		private void CreateRacesAndFactions(List<FactionPreset> presets)
		{
			var uniqueRacePresets = new Dictionary<RacePreset, int>();
			foreach(var preset in presets)
			{
				if(!uniqueRacePresets.Keys.Contains(preset.race))
				{
					uniqueRacePresets.Add(preset.race, 1);
				}
				else
				{
					uniqueRacePresets[preset.race] += 1;
				}
			}

			foreach(var racePresetPair in uniqueRacePresets)
			{
				var race = new Race(racePresetPair.Key);
				EventManager.Instance.Dispatch(new AddContextEvent(race));
				for (var i = 0; i < racePresetPair.Value; i++)
				{
					var faction = new Faction(1, 1000, race);
					EventManager.Instance.Dispatch(new AddContextEvent(faction));
				}
			}
		}

		override public void UpdateContext()
		{
			Age += 1;
			NumIncidents = 1;
		}

		override public void DeployContext()
		{
			IncidentService.Instance.PerformIncidents(this);
		}

		override public void Die() { }

		private int GetNextID()
		{
			var next = nextID;
			nextID++;
			return next;
		}

		public override void LoadContextProperties()
		{
			AllContexts.LoadContextProperties();
			CurrentContexts = new IncidentContextDictionary();

			foreach(var id in contextIDLoadBuffers["CurrentContexts"])
			{
				var context = AllContexts.GetContextByID(id);
				CurrentContexts[context.ContextType].Add(context);
			}

			LostItems = SaveUtilities.ConvertIDsToContexts<Item>(contextIDLoadBuffers["LostItems"]);
		}

		public override void CheckForDeath()
		{
			
		}
	}
}
