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
