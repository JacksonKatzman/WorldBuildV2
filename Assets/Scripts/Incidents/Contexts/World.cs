using Game.Factions;
using Game.Incidents;
using Game.Terrain;
using System;

namespace Game.Simulation
{
	public class World : IIncidentContext
	{
		[NonSerialized]
		private HexGrid hexGrid;

		public TypeListDictionary<IIncidentContext> Contexts { get; private set; }

		public Type ContextType => typeof(World);

		public int NumIncidents { get; set; }

		public int ParentID => -1;

		public int Age { get; set; }

		public int NumPeople => Contexts[typeof(Person)].Count;

		public World()
		{
			Contexts = new TypeListDictionary<IIncidentContext>();
		}

		public World(HexGrid hexGrid)
		{
			this.hexGrid = hexGrid;
			Age = 0;

			Contexts = new TypeListDictionary<IIncidentContext>();
		}

		public void Initialize()
		{
			CreateFactions(1);
		}

		public void AdvanceTime()
		{
			UpdateContext();
			foreach(var contextList in Contexts.Values)
			{
				foreach(var context in contextList)
				{
					context.UpdateContext();
				}
			}

			DeployContext();
			foreach (var contextList in Contexts.Values)
			{
				foreach (var context in contextList)
				{
					context.DeployContext();
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
			Contexts[typeof(T)].Add(context);
		}

		public void RemoveContext<T>(T context) where T : IIncidentContext
		{
			Contexts[typeof(T)].Remove(context);
		}

		private void CreateFactions(int numFactions)
		{
			for(int i = 0; i < numFactions; i++)
			{
				var faction = new Faction(1);
				Contexts[typeof(Faction)].Add(faction);
			}
		}

		public void UpdateContext()
		{
			Age += 1;
			NumIncidents = 1;
		}

		public void DeployContext()
		{
			IncidentService.Instance.PerformIncidents(this);
		}
	}
}
