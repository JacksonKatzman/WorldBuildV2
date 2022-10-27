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

		public ContextTypeListDictionary<IIncidentContextProvider> Providers { get; private set; }

		public World()
		{
			Providers = new ContextTypeListDictionary<IIncidentContextProvider>();
		}

		public World(HexGrid hexGrid)
		{
			this.hexGrid = hexGrid;
			Providers = new ContextTypeListDictionary<IIncidentContextProvider>();
		}

		public void Initialize()
		{
			CreateFactions(1);
		}

		public void AdvanceTime()
		{
			foreach(var providerList in Providers.Values)
			{
				foreach(var provider in providerList)
				{
					provider.UpdateContext();
					provider.DeployContext();
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
				Providers[typeof(FactionContext)].Add(faction);
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
