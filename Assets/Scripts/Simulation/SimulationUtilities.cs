using Game.Collections;
using Game.Incidents;
using Game.Terrain;
using Game.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Game.Simulation
{
	public static class SimulationUtilities
	{
		public static int GetRandomCellIndex(List<BiomeTerrainType> biomeTypes = null)
		{
			return GetRandomCell(biomeTypes).Index;
		}

		public static bool GetRandomUnclaimedCellIndex(out int index, List<BiomeTerrainType> biomeTypes = null, bool landTile = true)
		{
			var grid = SimulationManager.Instance.HexGrid;
			var claimedList = new List<int>();
			foreach (var context in ContextDictionaryProvider.CurrentContexts[typeof(Faction)])
			{
				claimedList.AddRange(((Faction)context).ControlledTileIndices);
			}
			var unclaimed = grid.cells.Where(x => !claimedList.Contains(x.Index)).ToList();
			if(landTile)
			{
				unclaimed = unclaimed.Where(x => !x.IsUnderwater).ToList();
			}
			var matches = ((biomeTypes != null && biomeTypes.Count > 0) ? unclaimed.Where(x => biomeTypes.Contains(x.TerrainType)) : unclaimed).ToList();
			if (matches.Count > 0)
			{
				index = matches[SimRandom.RandomRange(0, matches.Count)].Index;
				return true;
			}
			else
			{
				index = 0;
				return false;
			}
		}

		public static List<HexCell> GetAllCellsWithDistanceFromCity(int minDistance, int maxDistance, bool onLand = true)
		{
			var grid = SimulationManager.Instance.HexGrid;
			var allCells = grid.cells.ToList();
			if(onLand)
			{
				allCells = allCells.Where(x => !x.IsUnderwater).ToList();
			}
			var cityCellsIndices = GetCellsWithCities();
			var cityCells = new List<HexCell>();
			var results = new List<HexCell>();
			foreach(var index in cityCellsIndices)
			{
				cityCells.Add(grid.GetCell(index));
			}
			foreach(var cell in allCells)
			{
				var valid = true;
				foreach(var cityCell in cityCells)
				{
					var distance = cell.coordinates.DistanceTo(cityCell.coordinates);
					if(distance < minDistance || distance > maxDistance)
					{
						valid = false;
						break;
					}
				}
				if(valid)
				{
					results.Add(cell);
				}
			}

			return results;
		}

		public static List<HexCell> SecondTry(int minDistance, bool onLand = true)
		{
			var grid = SimulationManager.Instance.HexGrid;
			var allCells = new HashSet<HexCell>();
			foreach(var cell in grid.cells)
			{
				if(cell.IsUnderwater != onLand)
				{
					allCells.Add(cell);
				}
			}

			var cityCellsIndices = GetCellsWithCities();
			var cityCells = new HashSet<HexCell>();

			foreach (var index in cityCellsIndices)
			{
				var cityCell = grid.GetCell(index);
				cityCells.Add(cityCell);
				var inRange = GetAllCellsInRange(cityCell, minDistance);
				foreach(var inRangeCell in inRange)
				{
					cityCells.Add(inRangeCell);
				}
			}

			foreach(var cityCell in cityCells)
			{
				allCells.Remove(cityCell);
			}

			return allCells.ToList();
		}

		public static bool GetRandomEmptyCellIndex(out int index, List<BiomeTerrainType> biomeTypes = null)
		{
			var grid = SimulationManager.Instance.HexGrid;
			var claimedList = GetClaimedCells();

			var unclaimed = grid.cells.Where(x => !claimedList.Contains(x.Index)).ToList();
			var matches = ((biomeTypes != null && biomeTypes.Count > 0) ? unclaimed.Where(x => biomeTypes.Contains(x.TerrainType)) : unclaimed).ToList();
			if (matches.Count > 0)
			{
				index = matches[SimRandom.RandomRange(0, unclaimed.Count)].Index;
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
			foreach (var faction in ContextDictionaryProvider.CurrentContexts[typeof(Faction)])
			{
				var context = faction as Faction;
				if (context.ControlledTileIndices.Contains(index))
				{
					return false;
				}
			}

			return true;
		}

		public static bool IsCellUnclaimed(int index)
		{
			var claimedCells = GetClaimedCells();
			return !claimedCells.Contains(index);
		}

		private static HexCell GetRandomCell(List<BiomeTerrainType> biomeTypes = null)
		{
			var grid = SimulationManager.Instance.HexGrid;
			//var numCells = grid.cellCountX * grid.cellCountZ;
			var matches = ((biomeTypes != null && biomeTypes.Count > 0) ? grid.cells.Where(x => biomeTypes.Contains(x.TerrainType)) : grid.cells).ToList();
			return matches[SimRandom.RandomRange(0, matches.Count)];
		}

		public static List<int> GetClaimedCells()
		{
			var claimedList = new List<int>();
			foreach (var faction in ContextDictionaryProvider.CurrentContexts[typeof(Faction)])
			{
				var context = faction as Faction;
				claimedList.AddRange(context.ControlledTileIndices);
			}

			return claimedList;
		}

		public static List<HexCell> GetClaimedCellsAsHexCells()
		{
			var cellIndices = GetClaimedCells();
			var cells = new List<HexCell>();
			var grid = SimulationManager.Instance.HexGrid;
			foreach(var index in cellIndices)
			{
				cells.Add(grid.GetCell(index));
			}
			return cells;
		}

		public static List<int> GetCellsWithCities()
		{
			var claimedList = new List<int>();
			foreach (var city in ContextDictionaryProvider.CurrentContexts[typeof(City)])
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

		public static List<int> FindCitylessBorderWithinFaction(Faction faction, List<BiomeTerrainType> biomeTypes = null)
		{
			var cityTiles = GetCellsWithCities();
			var borderTiles = FindBorderWithinFaction(faction);
			List<int> possibleIndices = new List<int>();

			foreach (var cell in borderTiles)
			{
				if (biomeTypes != null && biomeTypes.Count > 0 && !biomeTypes.Contains(SimulationManager.Instance.HexGrid.cells[cell].TerrainType))
				{
					continue;
				}
				if (!cityTiles.Contains(cell))
				{
					possibleIndices.Add(cell);
				}
			}

			return possibleIndices;
		}

		public static List<int> FindBorderOutsideFaction(Faction faction, List<BiomeTerrainType> biomeTypes = null)
		{
			var insideIndices = FindBorderWithinFaction(faction);
			var possibleIndices = new HashSet<int>();
			var controlledCells = faction.ControlledTileIndices;

			foreach (var cell in insideIndices)
			{
				HexCell hexCell = SimulationManager.Instance.HexGrid.GetCell(cell);
				if (biomeTypes != null && biomeTypes.Count > 0 && !biomeTypes.Contains(hexCell.TerrainType))
				{
					continue;
				}
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

		public static List<int> FindSharedBorderFaction(Faction faction, List<BiomeTerrainType> biomeTypes = null)
		{
			var outsideIndices = FindBorderOutsideFaction(faction, biomeTypes);
			var possibleIndices = new List<int>();
			var claimedCells = GetClaimedCells();

			foreach (var cell in outsideIndices)
			{
				if (biomeTypes != null && biomeTypes.Count > 0 && !biomeTypes.Contains(SimulationManager.Instance.HexGrid.cells[cell].TerrainType))
				{
					continue;
				}
				if (claimedCells.Contains(cell))
				{
					possibleIndices.Add(cell);
				}
			}

			return possibleIndices;
		}

		public static List<int> FindSharedBorderFaction(Faction faction, Faction otherFaction, List<BiomeTerrainType> biomeTypes = null)
		{
			var outsideIndices = FindBorderOutsideFaction(faction, biomeTypes);
			var possibleIndices = new List<int>();
			var claimedCells = GetClaimedCells();

			foreach (var cell in outsideIndices)
			{
				if (biomeTypes != null && biomeTypes.Count > 0 && !biomeTypes.Contains(SimulationManager.Instance.HexGrid.cells[cell].TerrainType))
				{
					continue;
				}
				if (otherFaction.ControlledTileIndices.Contains(cell))
				{
					possibleIndices.Add(cell);
				}
			}

			return possibleIndices;
		}

		public static List<int> FindCitylessCellWithinFaction(Faction faction, int minDistanceFromCities = 1, List<BiomeTerrainType> biomeTypes = null)
		{
			List<int> possibleIndices = new List<int>();
			var cityTiles = GetCellsWithCities();
			foreach (var index in faction.ControlledTileIndices)
			{
				var cell = SimulationManager.Instance.HexGrid.cells[index];
				var valid = true;
				if(biomeTypes != null && biomeTypes.Count > 0 && !biomeTypes.Contains(cell.TerrainType))
				{
					continue;
				}
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

		public static List<HexCell> GetAllCellsInRange(HexCell fromCell, int range)
		{
			HexCellPriorityQueue searchFrontier = new HexCellPriorityQueue();
			List<HexCell> cellsInRange = ListPool<HexCell>.Get();

			var searchFrontierPhase = 2;
			if (searchFrontier == null)
			{
				searchFrontier = new HexCellPriorityQueue();
			}
			else
			{
				searchFrontier.Clear();
			}

			fromCell.SearchPhase = searchFrontierPhase;
			fromCell.Distance = 0;
			searchFrontier.Enqueue(fromCell);
			HexCoordinates fromCoordinates = fromCell.coordinates;
			while (searchFrontier.Count > 0)
			{
				HexCell current = searchFrontier.Dequeue();
				current.SearchPhase += 1;
				cellsInRange.Add(current);

				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
				{
					HexCell neighbor = current.GetNeighbor(d);
					if (
						neighbor == null ||
						neighbor.SearchPhase > searchFrontierPhase ||
						!neighbor.Explorable
					)
					{
						continue;
					}

					int distance = current.Distance + 1;
					if (distance > range ||
						distance > fromCoordinates.DistanceTo(neighbor.coordinates)
					)
					{
						continue;
					}

					if (neighbor.SearchPhase < searchFrontierPhase)
					{
						neighbor.SearchPhase = searchFrontierPhase;
						neighbor.Distance = distance;
						neighbor.SearchHeuristic = 0;
						searchFrontier.Enqueue(neighbor);
					}
					else if (distance < neighbor.Distance)
					{
						int oldPriority = neighbor.SearchPriority;
						neighbor.Distance = distance;
						searchFrontier.Change(neighbor, oldPriority);
					}
				}
			}

			SimulationManager.Instance.HexGrid.ResetSearchPhases();

			return cellsInRange;
		}

		public static HexCell GetCentroid(List<int> cellIds)
		{
			var points = new List<HexCell>();
			foreach (var id in cellIds)
			{
				points.Add(SimulationManager.Instance.HexGrid.GetCell(id));
			}
			return GetCentroid(points);
		}

		public static HexCell GetCentroid(List<HexCell> points)
		{
			var x = 0;
			var z = 0;
			foreach(var point in points)
			{
				x += point.coordinates.X;
				z += point.coordinates.Z;
			}

			int xCoord = x / points.Count;
			int zCoord = z / points.Count;

			return SimulationManager.Instance.HexGrid.cells.First(cell => cell.coordinates.X == xCoord && cell.coordinates.Z == zCoord);
		}

		public static List<HexCell> GetConvexHull(List<int> cellIds)
		{
			var points = new List<HexCell>();
			foreach (var id in cellIds)
			{
				points.Add(SimulationManager.Instance.HexGrid.GetCell(id));
			}
			return GetConvexHull(points);
		}

		public static List<HexCell> GetConvexHull(List<HexCell> points)
		{
			var result = new HashSet<HexCell>();
			int leftMostIndex = 0;
			for (int i = 1; i < points.Count; i++)
			{
				if (points[leftMostIndex].coordinates.X > points[i].coordinates.X)
					leftMostIndex = i;
			}
			result.Add(points[leftMostIndex]);
			List<HexCell> collinearPoints = new List<HexCell>();
			HexCell current = points[leftMostIndex];
			while (true)
			{
				HexCell nextTarget = points[0];
				for (int i = 1; i < points.Count; i++)
				{
					if (points[i] == current)
						continue;
					float x1, x2, y1, y2;
					x1 = current.coordinates.X - nextTarget.coordinates.X;
					x2 = current.coordinates.X - points[i].coordinates.X;

					y1 = current.coordinates.Z - nextTarget.coordinates.Z;
					y2 = current.coordinates.Z - points[i].coordinates.Z;

					float val = (y2 * x1) - (y1 * x2);
					if (val > 0)
					{
						nextTarget = points[i];
						collinearPoints = new List<HexCell>();
					}
					else if (val == 0)
					{
						//if (Vector2.Distance(current.position, nextTarget.position) < Vector2.Distance(current.position, points[i].position))
						if(current.coordinates.DistanceTo(nextTarget.coordinates) < current.coordinates.DistanceTo(points[i].coordinates))
						{
							collinearPoints.Add(nextTarget);
							nextTarget = points[i];
						}
						else
							collinearPoints.Add(points[i]);
					}
				}

				foreach (HexCell t in collinearPoints)
					result.Add(t);
				if (nextTarget == points[leftMostIndex])
					break;
				result.Add(nextTarget);
				current = nextTarget;
			}

			/*
			List<Vector2> convertedResult = new List<Vector2>();
			foreach (Transform transform in result)
			{
				convertedResult.Add(new Vector2(transform.position.x, transform.position.y));
			}
			polygon = new Polygon(convertedResult);
			*/
			return result.ToList();
		}
	}
}
