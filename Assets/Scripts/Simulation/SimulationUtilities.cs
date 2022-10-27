using Game.Factions;
using Game.Incidents;
using Game.Terrain;
using System.Collections.Generic;

namespace Game.Simulation
{
	public static class SimulationUtilities
	{
		public static int GetRandomCellIndex()
		{
			return GetRandomCell().Index;
		}

		public static bool GetRandomUnclaimedCellIndex(out int index)
		{
			var found = false;
			var cellIndex = 0;
			index = -1;

			for (int i = 0; i < 250 && !found; i++)
			{
				cellIndex = GetRandomCellIndex();
				found = IsCellIndexUnclaimed(cellIndex);
			}

			if(found)
			{
				index = cellIndex;
			}

			return found;
		}

		public static bool IsCellIndexUnclaimed(int index)
		{
			foreach (var faction in SimulationManager.Instance.world.Providers[typeof(FactionContext)])
			{
				var context = faction.GetContext() as FactionContext;
				if (context.controlledTileIndices.Contains(index))
				{
					return false;
				}
			}

			return true;
		}

		private static HexCell GetRandomCell()
		{
			var grid = SimulationManager.Instance.HexGrid;
			var numCells = grid.cellCountX * grid.cellCountZ;
			return grid.GetCell(SimRandom.RandomRange(0, numCells));
		}
	}
}
