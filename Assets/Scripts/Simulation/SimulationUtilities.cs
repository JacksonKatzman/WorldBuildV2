using Game.Factions;
using Game.Incidents;
using Game.Terrain;
using System.Collections.Generic;
using System.Linq;

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
			var grid = SimulationManager.Instance.HexGrid;
			var claimedList = new List<int>();
			foreach (var context in SimulationManager.Instance.world.Contexts[typeof(Faction)])
			{
				claimedList.AddRange(((Faction)context).ControlledTileIndices);
			}
			var unclaimed = grid.cells.Where(x => !claimedList.Contains(x.Index)).ToList();
			if (unclaimed.Count > 0)
			{
				index = unclaimed[SimRandom.RandomRange(0, unclaimed.Count)].Index;
				return true;
			}
			else
			{
				index = 0;
				return false;
			}
		}

		public static bool GetRandomEmptyCellIndex(out int index)
		{
			var grid = SimulationManager.Instance.HexGrid;
			var claimedList = GetClaimedCells();

			var unclaimed = grid.cells.Where(x => !claimedList.Contains(x.Index)).ToList();
			if (unclaimed.Count > 0)
			{
				index = unclaimed[SimRandom.RandomRange(0, unclaimed.Count)].Index;
				return true;
			}
			else
			{
				index = 0;
				return false;
			}
		}

		public static List<int> GetEmptyTilesFromList(List<int> tiles)
		{
			var claimedList = GetClaimedCells();
			return tiles.Where(x => !claimedList.Contains(x)).ToList();
		}

		public static List<int> GetCitylessTilesFromList(List<int> tiles)
		{
			var claimedList = GetCellsWithCities();
			return tiles.Where(x => !claimedList.Contains(x)).ToList();
		}

		public static bool IsCellIndexUnclaimed(int index)
		{
			foreach (var faction in SimulationManager.Instance.world.Contexts[typeof(Faction)])
			{
				var context = faction as Faction;
				if (context.ControlledTileIndices.Contains(index))
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

		public static List<int> GetClaimedCells()
		{
			var claimedList = new List<int>();
			foreach (var pair in SimulationManager.Instance.world.Contexts)
			{
				if (pair.Key.IsAssignableFrom(typeof(ILocationAffiliated)))
				{
					foreach (var context in pair.Value)
					{
						claimedList.Add(((ILocationAffiliated)context).CurrentLocation.TileIndex);
					}
				}
			}

			return claimedList;
		}

		public static List<int> GetCellsWithCities()
		{
			var claimedList = new List<int>();
			foreach (var city in SimulationManager.Instance.world.Contexts[typeof(City)])
			{
				claimedList.Add(((City)city).CurrentLocation.TileIndex);
			}

			return claimedList;
		}
	}
}
