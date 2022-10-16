using Game.Factions;
using Game.Incidents;
using Game.IO;
using Game.Terrain;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Game.Simulation
{
	public class World
	{
		[NonSerialized]
		private HexGrid hexGrid;

		public static TypeListDictionary<IIncidentContextProvider> Providers { get; private set; }

		public World() { }

		public World(HexGrid hexGrid)
		{
			this.hexGrid = hexGrid;
			Providers = new TypeListDictionary<IIncidentContextProvider>();

			CreateFactions(1);
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
				Providers[typeof(Faction)].Add(new Faction());
			}
		}
	}

	public class TypeListDictionary<T> : Dictionary<Type, List<T>>
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
