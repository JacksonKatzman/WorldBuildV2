using Game.IO;
using Game.Terrain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Simulation
{
	public class World
	{
		[NonSerialized]
		private HexGrid hexGrid;

		private SimulationCellContainer cellContainer;

		public World() { }

		public World(HexGrid hexGrid)
		{
			this.hexGrid = hexGrid;
			cellContainer = new SimulationCellContainer(hexGrid.cells);
		}

		public void Save(string mapName)
		{
			cellContainer.Save(mapName);
		}

		public static World Load(HexGrid hexGrid, string mapName)
		{
			var world = new World();
			world.hexGrid = hexGrid;

			world.cellContainer = SimulationCellContainer.Load(hexGrid, mapName);

			return world;
		}
	}
}
