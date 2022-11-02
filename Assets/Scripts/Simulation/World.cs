using Game.Factions;
using Game.Incidents;
using Game.Terrain;
using System;
using System.Collections.Generic;

namespace Game.Simulation
{
	public class World
	{
		[NonSerialized]
		private HexGrid hexGrid;

		public ContextTypeListDictionary<IIncidentContext> Contexts { get; private set; }

		public World()
		{
			Contexts = new ContextTypeListDictionary<IIncidentContext>();
		}

		public World(HexGrid hexGrid)
		{
			this.hexGrid = hexGrid;

			Contexts = new ContextTypeListDictionary<IIncidentContext>();
		}

		public void Initialize()
		{
			CreateFactions(1);
		}

		public void AdvanceTime()
		{
			foreach(var contextList in Contexts.Values)
			{
				foreach(var context in contextList)
				{
					context.UpdateContext();
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

		private void CreateFactions(int numFactions)
		{
			for(int i = 0; i < numFactions; i++)
			{
				var faction = new Faction();
				Contexts[typeof(Faction)].Add(faction);
				faction.AttemptExpandBorder(1);
			}
		}
	}

	public class ContextTypeListDictionary<T> : Dictionary<Type, List<T>>
	{
		public new List<T> this[Type key]
		{
			get
			{
				if(!this.ContainsKey(key))
				{
					this.Add(key, new List<T>());
				}

				return base[key];
			}
			set
			{
				if (!this.ContainsKey(key))
				{
					this.Add(key, new List<T>());
				}

				base[key] = value;
			}
		}
	}
}
