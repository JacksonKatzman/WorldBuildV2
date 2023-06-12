using Game.Factions;
using Game.Incidents;
using Game.Terrain;
using Game.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

namespace Game.Simulation
{
	public class World : IncidentContext
	{
		[NonSerialized]
		private HexGrid hexGrid;

		[JsonProperty, ES3Serializable]
		public IncidentContextDictionary CurrentContexts { get; private set; }
		[JsonProperty, ES3Serializable]
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
		public int NumPeople => CurrentContexts[typeof(Character)].Count;

		public int nextID;

		public World()
		{
			//CurrentContexts = new TypeListDictionary<IIncidentContext>();
			EventManager.Instance.AddEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
		}

		public World(HexGrid hexGrid) : this()
		{
			this.hexGrid = hexGrid;
			nextID = ID + 1;
			Age = 0;

			CurrentContexts = new IncidentContextDictionary();
			AllContexts = new IncidentContextDictionary();
			contextsToAdd = new IncidentContextDictionary();
			contextsToRemove = new IncidentContextDictionary();
		}

		public void Initialize(List<FactionPreset> factions)
		{
			CreateRacesAndFactions(factions);
		}

		public void AdvanceTime()
		{
			DelayedRemoveContexts();
			DelayedAddContexts();

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
		}

		public void BeginPostGeneration()
		{
			foreach(var faction in Factions)
			{
				GenerateAdditionalCities(faction);

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
				var ordered = possibleTiles.OrderByDescending(x => hexGrid.GetCell(x).CalculateInhabitability());
				var chosenLocationIndex = ordered.First();
				var population = SimRandom.RandomRange((int)(faction.Cities[0].Population * 0.3f), (int)(faction.Cities[0].Population * 0.7f));
				var createdCity = new City(faction, new Location(chosenLocationIndex), population, 0);
				AddContext(createdCity);
				DelayedAddContexts();
			}
		}

		public void DrawCities()
		{
			foreach(var city in Cities)
			{
				var location = city.CurrentLocation.TileIndex;
				var tile = hexGrid.GetCell(location);

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
			var borderCells = SimulationUtilities.FindBorderWithinFaction(faction);

			foreach (var index in borderCells)
			{
				var cell = hexGrid.GetCell(index);
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
						cell.hexCellLabel.ToggleBorder(d, true);
					}
				}
			}
		}

		public void Save(string mapName)
		{
			SaveUtilities.SerializeSave(this, SaveUtilities.GetWorldPath(mapName));
		}

		public static World Load(HexGrid hexGrid, string mapName)
		{
			//var world = new World();
			var world = SaveUtilities.SerializeLoad<World>(SaveUtilities.GetMapRootPath(mapName), "World.json");
			world.hexGrid = hexGrid;

			return world;
		}

		public void AddContext<T>(T context) where T : IIncidentContext
		{
			context.ID = GetNextID();
			contextsToAdd[typeof(T)].Add(context);
		}

		public void AddContextImmediate<T>(T context) where T : IIncidentContext
		{
			AddContext(context);
			DelayedAddContexts();
		}

		public void RemoveContext<T>(T context) where T : IIncidentContext
		{
			contextsToRemove[typeof(T)].Add(context);
		}

		private void OnRemoveContextEvent(RemoveContextEvent gameEvent)
		{
			RemoveContext(gameEvent.context);
		}

		private void DelayedAddContexts()
		{
			foreach (var contextList in contextsToAdd.Values)
			{
				foreach (var context in contextList)
				{
					CurrentContexts[context.ContextType].Add(context);
					AllContexts[context.ContextType].Add(context);
				}
				contextList.Clear();
			}
		}

		private void DelayedRemoveContexts()
		{
			foreach (var contextList in contextsToRemove.Values)
			{
				foreach (var context in contextList)
				{
					CurrentContexts[context.ContextType].Remove(context);
				}
				contextList.Clear();
			}
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
				AddContext(race);
				for (var i = 0; i < racePresetPair.Value; i++)
				{
					var faction = new Faction(1, 1000, race);
					AddContext(faction);
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
		}
	}
}
