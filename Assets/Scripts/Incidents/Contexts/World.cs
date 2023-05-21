using Game.Factions;
using Game.Incidents;
using Game.Terrain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Game.Simulation
{
	public class World : IncidentContext
	{
		[NonSerialized]
		private HexGrid hexGrid;

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

		public List<Person> People => CurrentContexts[typeof(Person)].Cast<Person>().ToList();
		public List<Faction> Factions => CurrentContexts[typeof(Faction)].Cast<Faction>().ToList();
		public List<City> Cities => CurrentContexts[typeof(City)].Cast<City>().ToList();
		public int NumPeople => CurrentContexts[typeof(Person)].Count;

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
			//Generate towns/hamlets/villages around each of the existing cities
			foreach(var city in Cities)
			{
				var location = city.CurrentLocation.TileIndex;
				var tile = hexGrid.GetCell(location);
				tile.SpecialIndex = 1;
			}

			foreach(var faction in Factions)
			{
				var numCities = faction.NumCities;
				var numTowns = numCities * 2; //temporary calc
				for(int i = 0; i < numTowns; i++)
				{
					var possibleTiles = SimulationUtilities.FindCitylessCellWithinFaction(faction, 2);
					var ordered = possibleTiles.OrderByDescending(x => hexGrid.GetCell(x).CalculateInhabitability());
					var chosenLocationIndex = ordered.First();
					var createdCity = new City(faction, new Location(chosenLocationIndex), 100, 0);
					AddContext(createdCity);
					DelayedAddContexts();

					hexGrid.GetCell(chosenLocationIndex).SpecialIndex = 2;
				}

				var borderCells = SimulationUtilities.FindBorderWithinFaction(faction);

				foreach(var index in borderCells)
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
			//Calc how many towns to make
			//Get citiless tiles within faction using min distance
			//find one with best inhabitability or w/e
			//plop town and repeat for # towns to make



			//Pick location for players to start, likely in one of the towns/hamlets
			//Generate layout of town/what its contents is
			//Generate all the points of interest/people of interest in the town
			//Generate NPCs for the tavern the players start in
			//Generate adventure based in location/people of interest etc
			//Extra credit: generate world points of interest in case players want to explore for their adventures instead?
			//That or just include exploration contracts among the possible adventures
		}

		public void Save(string mapName)
		{

		}

		public static World Load(HexGrid hexGrid, string mapName)
		{
			var world = new World();
			world.hexGrid = hexGrid;

			return world;
		}

		public void AddContext<T>(T context) where T : IIncidentContext
		{
			context.ID = GetNextID();
			contextsToAdd[typeof(T)].Add(context);
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
					var faction = new Faction(1, race);
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
	}
}
