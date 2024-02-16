using Cysharp.Threading.Tasks;
using Game.Data;
using Game.Debug;
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
using UnityEngine.Profiling;

namespace Game.Simulation
{
	public static class WorldExtensions
	{
		public static float SimulationCompletionPercentage(this World world)
		{
			return (((float)world.Age) / ((float)world.simulationOptions.simulatedYears));
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
			
			//return SimulationCompletionPercentage(world) >= (SimulationUtilities.GetClaimedCells().Count / (world.HexGrid.cells.Count() * world.simulationOptions.claimedHexPercentage));
			
			var claimedCells = SimulationUtilities.GetClaimedCells();
			var worldCells = world.HexGrid.cells.ToList();
			var landCells = worldCells.Where(x => !x.IsUnderwater).ToList();
			var allowedPercentage = world.simulationOptions.claimedHexPercentage;
			var allowedToBeClaimed = landCells.Count * allowedPercentage;
			var percentageClaimed = claimedCells.Count / allowedToBeClaimed;
			var completionPercentage = SimulationCompletionPercentage(world);
			return completionPercentage >= percentageClaimed;
			
			//return true;
		}

		public static bool CanFillAdditionalOrganizationPosition(this World world, int age, int currentNumPositions)
		{
			var maxAge = world.simulationOptions.simulatedYears;
			var interval = maxAge / world.MaxOrganizationPositions;

			return age >= (interval * currentNumPositions);
		}
	}
	public class World : IncidentContext
	{
		public static World CurrentWorld => SimulationManager.Instance.world;
		public HexGrid HexGrid => SimulationManager.Instance.HexGrid;

		public IncidentContextDictionary CurrentContexts { get; private set; }
		public IncidentContextDictionary AllContexts { get; private set; }
		private IncidentContextDictionary contextsToAdd;
		private IncidentContextDictionary contextsToRemove;

		public bool PostSimulationCompleted => postSimulationCompleted;
		private bool postSimulationCompleted;

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

		[JsonIgnore]
		public List<Character> People => CurrentContexts[typeof(Character)].Cast<Character>().ToList();
		[JsonIgnore]
		public List<Faction> Factions => CurrentContexts[typeof(Faction)].Cast<Faction>().ToList();
		[JsonIgnore]
		public List<City> Cities => CurrentContexts[typeof(City)].Cast<City>().ToList();
		[JsonIgnore]
		public List<Landmark> Landmarks => CurrentContexts[typeof(Landmark)].Cast<Landmark>().ToList();
		[JsonIgnore]
		public List<GreatMonster> GreatMonsters => CurrentContexts[typeof(GreatMonster)].Cast<GreatMonster>().ToList();
		[JsonIgnore]
		public int NumPeople => People.Count;
		[JsonIgnore]
		public bool RoomForPeople => this.ShouldIncreaseCharacters();
		[JsonIgnore]
		public int NumFactions => Factions.Count;
		[JsonIgnore]
		public bool RoomForFactions => this.ShouldIncreaseFactions();
		[JsonIgnore]
		public int NumSpecialFactions => Factions.Where(x => x.IsSpecialFaction).Count();
		[JsonIgnore]
		public bool RoomForSpecialFaction => this.ShouldIncreaseSpecialFactions();
		[JsonIgnore]
		public int NumGreatMonsters => GreatMonsters.Count;
		[JsonIgnore]
		public int NumGreatDemons => GreatMonsters.Where(x => x.CreatureType == Enums.CreatureType.FIEND).Count();
		[JsonIgnore]
		public bool RoomForGreatMonsters => this.ShouldIncreaseGreatMonsters();
		public bool CanClaimMoreTerritory { get; set; }
		public int MaxOrganizationPositions { get; private set; }

		public List<Item> LostItems;

		public SimulationOptions simulationOptions;

		public int nextID;

		public World()
		{
			nextID = ID + 1;
			Age = 0;
			MaxOrganizationPositions = 1;

			CurrentContexts = new IncidentContextDictionary();
			AllContexts = new IncidentContextDictionary();
			contextsToAdd = new IncidentContextDictionary();
			contextsToRemove = new IncidentContextDictionary();
			LostItems = new List<Item>();
		}

		public void Initialize(SimulationOptions options)
		{
			simulationOptions = options;
			ContextDictionaryProvider.AllowImmediateChanges = true;
			CreateRacesAndFactions();

			/*
			var hexCollections = ContextDictionaryProvider.GetAllContexts<HexCollection>();
			OutputLogger.Log($"Total HexCollections: {hexCollections.Count}");
			var totalSize = hexCollections[0].cellCollection.Count;
			var biggest = hexCollections[0];
			for (int i = 1; i < hexCollections.Count; i++)
			{
				var collection = hexCollections[i];
				totalSize += collection.cellCollection.Count;
				if(collection.cellCollection.Count > biggest.cellCollection.Count)
				{
					biggest = collection;
				}

				foreach(var cellIndex in collection.cellCollection)
				{
					//var cell = HexGrid.GetCell(cellIndex);
					//cell.hexCellLabel.SetText(i.ToString());
				}
			}

			//this is temporary to test
			//ALSO need to fix the ui portion, name isnt swapping as fast as i can mouse over things
			//also RIVERS?
			foreach(var collection in hexCollections)
			{
				var closestFaction = SimulationUtilities.GetCityNearestLocation(collection.CurrentLocation).AffiliatedFaction;
				collection.Name = closestFaction.namingTheme.GenerateTerrainName(collection.CollectionType, collection.AffiliatedTerrainType);
			}

			OutputLogger.Log($"Biggest Collection: {biggest.cellCollection.Count}:{biggest.AffiliatedTerrainType} - Average Size: {totalSize / hexCollections.Count}");
			*/
		}

		public async UniTask AdvanceTime()
		{
			//GameProfiler.worldUpdateMarker.Begin();
			ContextDictionaryProvider.DelayedRemoveContexts();
			ContextDictionaryProvider.DelayedAddContexts();
			ContextDictionaryProvider.AllowImmediateChanges = false;

			CanClaimMoreTerritory = this.ShouldClaimMoreTerritory();

			UpdateContext();
			foreach(var contextList in CurrentContexts.Values)
			{
				foreach(var context in contextList)
				{
					context.UpdateContext();
				}
			}

			//GameProfiler.BeginProfiling(typeof(World).ToString(), GameProfiler.ProfileFunctionType.DEPLOY);
			DeployContext();
			foreach (var contextList in CurrentContexts)
			{
				if(contextList.Key == typeof(Character))
					GameProfiler.BeginProfiling(contextList.Key.ToString(), GameProfiler.ProfileFunctionType.DEPLOY);
				foreach (var context in contextList.Value)
				{
					context.DeployContext();
				}
				if (contextList.Key == typeof(Character))
					GameProfiler.EndProfiling(contextList.Key.ToString());
			}
			//GameProfiler.EndProfiling(typeof(World).ToString());

			/*
			UpdateHistoricalData();
			foreach (var contextList in CurrentContexts.Values)
			{
				foreach (var context in contextList)
				{
					context.UpdateHistoricalData();
				}
			}
			*/

			//GameProfiler.worldUpdateMarker.End();

			ContextDictionaryProvider.AllowImmediateChanges = true;
			await UniTask.Yield();
		}

		public async UniTask HandlePostSimulation()
        {
			PostSimulationCleanup();
			BeginPostGeneration();
			postSimulationCompleted = true;
			await UniTask.Yield();
		}

		private void PostSimulationCleanup()
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

		private void BeginPostGeneration()
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

			if (!SimulationManager.Instance.simulationOptions.DrawFeaturesBeforeSimulation)
			{
				//connect all major cities
				//connect SOME of the smaller ones
				AddRoads();
				//DrawCities();
				//draw features here
				DrawCities();
				DrawLandmarks();
				DrawFeatures();
			}

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
				EventManager.Instance.Dispatch(new AddContextEvent(createdCity, true));
			}
		}

		public void AddRoads()
        {
			for(int i = 0; i < Cities.Count; i++)
            {
				var startingCity = Cities[0];
				for(int j = 0; j < Cities.Count; j++)
                {
					var toCity = Cities[j];
					if(toCity == startingCity)
                    {
						continue;
                    }
					HexGrid.FindPathForRoad(startingCity.GetHexCell(), toCity.GetHexCell());
					if (HexGrid.HasPath)
					{
						var path = HexGrid.GetPath();
						for (int k = 0; k < path.Count - 1; k++)
						{
							var cell = path[k];
							if (cell.IsNeighbor(path[k + 1], out var direction))
							{
								
								var neighbor = path[k + 1];
								HexEdgeType edgeType = cell.GetEdgeType(neighbor);
								if(edgeType == HexEdgeType.Cliff)
                                {
									if(neighbor.Elevation > cell.Elevation)
                                    {
										neighbor.Elevation = cell.Elevation + 1;
                                    }
									else
                                    {
										neighbor.Elevation = cell.Elevation - 1;
                                    }
                                }
								
								cell.AddRoad(direction);
							}
						}
					}
                }
            }
        }

		public void DrawLandmarks()
        {
			foreach(var landmark in Landmarks)
            {
				var location = landmark.CurrentLocation.TileIndex;
				var cell = HexGrid.GetCell(location);
				cell.chunk.features.AddLandmark(landmark, cell, cell.Position);
            }
        }

		public void DrawCities()
		{
			foreach(var city in Cities)
			{
				var location = city.CurrentLocation.TileIndex;
				var cell = HexGrid.GetCell(location);
				cell.chunk.features.AddCity(city, cell, cell.Position);		
			}
		}

		public void DrawFeatures()
        {
			foreach (var chunk in HexGrid.chunks)
			{
				chunk.AddFeatures();
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

		private void CreateRacesAndFactions()
		{
			var presets = simulationOptions.factions;
			var uniqueRacePresets = new Dictionary<RacePreset, int>();
			foreach(var preset in presets)
			{
				if(!uniqueRacePresets.Keys.Contains(preset.Key.race))
				{
					uniqueRacePresets.Add(preset.Key.race, preset.Value);
				}
				else
				{
					uniqueRacePresets[preset.Key.race] += preset.Value;
				}
			}

			var postCreationMaxTotalPositions = int.MaxValue;

			foreach(var racePresetPair in uniqueRacePresets)
			{
				var race = new Race(racePresetPair.Key);
				EventManager.Instance.Dispatch(new AddContextEvent(race, true));
				for (var i = 0; i < racePresetPair.Value; i++)
				{
					var faction = new Faction(1, 10000, race, true);
					EventManager.Instance.Dispatch(new AddContextEvent(faction, true));

					if(faction.Government.TotalPositions < postCreationMaxTotalPositions)
					{
						postCreationMaxTotalPositions = faction.Government.MaxPositionsFilledInSimulation;
					}
				}
			}

			MaxOrganizationPositions = postCreationMaxTotalPositions;
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
