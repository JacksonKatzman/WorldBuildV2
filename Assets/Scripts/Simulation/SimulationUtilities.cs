using Game.Incidents;
using Game.Terrain;
using Game.Utilities;
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
			foreach (var context in SimulationManager.Instance.world.CurrentContexts[typeof(Faction)])
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

		public static List<int> GetEmptyCellsFromList(List<int> tiles)
		{
			var claimedList = GetClaimedCells();
			return tiles.Where(x => !claimedList.Contains(x)).ToList();
		}

		public static List<int> GetCitylessCellsFromList(List<int> tiles)
		{
			var claimedList = GetCellsWithCities();
			return tiles.Where(x => !claimedList.Contains(x)).ToList();
		}

		public static bool IsCellIndexUnclaimed(int index)
		{
			foreach (var faction in SimulationManager.Instance.world.CurrentContexts[typeof(Faction)])
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
			foreach (var pair in SimulationManager.Instance.world.CurrentContexts)
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
			foreach (var city in SimulationManager.Instance.world.CurrentContexts[typeof(City)])
			{
				claimedList.Add(((City)city).CurrentLocation.TileIndex);
			}

			return claimedList;
		}

		public static List<int> FindBorderWithinFaction(Faction faction)
		{
			List<int> possibleIndices = new List<int>();
			var controlledCells = faction.ControlledTileIndices;

			foreach (var cell in controlledCells)
			{
				HexCell hexCell = SimulationManager.Instance.HexGrid.GetCell(cell);
				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
				{
					HexCell neighbor = hexCell.GetNeighbor(d);
					if (neighbor != null && !controlledCells.Contains(neighbor.Index))
					{
						possibleIndices.Add(hexCell.Index);
						break;
					}
				}
			}

			return possibleIndices;
		}

		public static List<int> FindCitylessBorderWithinFaction(Faction faction)
		{
			var cityTiles = GetCellsWithCities();
			var borderTiles = FindBorderWithinFaction(faction);
			List<int> possibleIndices = new List<int>();

			foreach (var cell in borderTiles)
			{
				if (!cityTiles.Contains(cell))
				{
					possibleIndices.Add(cell);
				}
			}

			return possibleIndices;
		}

		public static List<int> FindBorderOutsideFaction(Faction faction)
		{
			var insideIndices = FindBorderWithinFaction(faction);
			var possibleIndices = new HashSet<int>();
			var controlledCells = faction.ControlledTileIndices;

			foreach (var cell in insideIndices)
			{
				HexCell hexCell = SimulationManager.Instance.HexGrid.GetCell(cell);
				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
				{
					HexCell neighbor = hexCell.GetNeighbor(d);
					if (neighbor != null && !controlledCells.Contains(neighbor.Index))
					{
						possibleIndices.Add(neighbor.Index);
					}
				}
			}

			return new List<int>(possibleIndices);
		}

		public static List<int> FindSharedBorderFaction(Faction faction)
		{
			var outsideIndices = FindBorderOutsideFaction(faction);
			var possibleIndices = new List<int>();
			var claimedCells = GetClaimedCells();

			foreach (var cell in outsideIndices)
			{
				if (claimedCells.Contains(cell))
				{
					possibleIndices.Add(cell);
				}
			}

			return possibleIndices;
		}

		public static List<int> FindCitylessCellWithinFaction(Faction faction, int minDistanceFromCities)
		{
			List<int> possibleIndices = new List<int>();
			var cityTiles = GetCellsWithCities();
			foreach (var index in faction.ControlledTileIndices)
			{
				var cell = SimulationManager.Instance.HexGrid.cells[index];
				var valid = true;
				foreach (var cityIndex in cityTiles)
				{
					var cityCell = SimulationManager.Instance.HexGrid.cells[cityIndex];
					if (cell.coordinates.DistanceTo(cityCell.coordinates) < minDistanceFromCities)
					{
						valid = false;
						break;
					}
				}
				if (valid)
				{
					possibleIndices.Add(index);
				}
			}

			return possibleIndices;
		}
	}
}
