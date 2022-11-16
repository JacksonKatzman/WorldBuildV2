using Game.Factions;
using Game.Incidents;
using Game.Terrain;
using System;
using System.Data;

namespace Game.Simulation
{
	public class World : IncidentContext
	{
		[NonSerialized]
		private HexGrid hexGrid;

		public TypeListDictionary<IIncidentContext> CurrentContexts { get; private set; }
		public TypeListDictionary<IIncidentContext> AllContexts { get; private set; }
		private TypeListDictionary<IIncidentContext> contextsToAdd;
		private TypeListDictionary<IIncidentContext> contextsToRemove;

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
			CurrentContexts = new TypeListDictionary<IIncidentContext>();
		}

		public World(HexGrid hexGrid)
		{
			this.hexGrid = hexGrid;
			nextID = ID + 1;
			Age = 0;

			CurrentContexts = new TypeListDictionary<IIncidentContext>();
			AllContexts = new TypeListDictionary<IIncidentContext>();
			contextsToAdd = new TypeListDictionary<IIncidentContext>();
			contextsToRemove = new TypeListDictionary<IIncidentContext>();
		}

		public void Initialize()
		{
			CreateFactions(5);
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
			//Contexts[typeof(T)].Add(context);
			context.ID = GetNextID();
			contextsToAdd[typeof(T)].Add(context);
		}

		public void RemoveContext<T>(T context) where T : IIncidentContext
		{
			//Contexts[typeof(T)].Remove(context);
			contextsToRemove[typeof(T)].Add(context);
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

		private void CreateFactions(int numFactions)
		{
			for(int i = 0; i < numFactions; i++)
			{
				var faction = new Faction(1);
				AddContext(faction);
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

		private int GetNextID()
		{
			var next = nextID;
			nextID++;
			return next;
		}
	}
}
