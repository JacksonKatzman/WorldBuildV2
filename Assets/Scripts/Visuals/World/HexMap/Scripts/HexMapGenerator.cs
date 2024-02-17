﻿using Game.Collections;
using Game.Debug;
using Game.Incidents;
using Game.Simulation;
//using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Terrain
{
	public class HexMapGenerator : MonoBehaviour
	{
		public HexGrid grid;

		public bool useFixedSeed;

		public int seed;

		[Range(0f, 0.5f)]
		public float jitterProbability = 0.25f;

		[Range(20, 200)]
		public int chunkSizeMin = 30;

		[Range(20, 200)]
		public int chunkSizeMax = 100;

		[Range(0f, 1f)]
		public float highRiseProbability = 0.25f;

		[Range(0f, 0.4f)]
		public float sinkProbability = 0.2f;

		[Range(5, 95)]
		public int landPercentage = 50;

		[Range(1, 5)]
		public int waterLevel = 3;

		[Range(-4, 0)]
		public int elevationMinimum = -2;

		[Range(6, 10)]
		public int elevationMaximum = 8;

		[Range(0, 10)]
		public int mapBorderX = 5;

		[Range(0, 10)]
		public int mapBorderZ = 5;

		[Range(0, 10)]
		public int regionBorder = 5;

		[Range(1, 4)]
		public int regionCount = 1;

		[Range(0, 100)]
		public int erosionPercentage = 50;

		[Range(0f, 1f)]
		public float startingMoisture = 0.1f;

		[Range(0f, 1f)]
		public float evaporationFactor = 0.5f;

		[Range(0f, 1f)]
		public float precipitationFactor = 0.25f;

		[Range(0f, 1f)]
		public float runoffFactor = 0.25f;

		[Range(0f, 1f)]
		public float seepageFactor = 0.125f;

		public HexDirection windDirection = HexDirection.NW;

		[Range(1f, 10f)]
		public float windStrength = 4f;

		[Range(0, 20)]
		public int riverPercentage = 10;

		[Range(0f, 1f)]
		public float extraLakeProbability = 0.25f;

		[Range(0f, 1f)]
		public float lowTemperature = 0f;

		[Range(0f, 1f)]
		public float highTemperature = 1f;

		public enum HemisphereMode
		{
			Both, North, South
		}

		public HemisphereMode hemisphere;

		[Range(0f, 1f)]
		public float temperatureJitter = 0.1f;

		HexCellPriorityQueue searchFrontier;

		int searchFrontierPhase;

		int cellCount, landCells;

		int temperatureJitterChannel;

		struct MapRegion
		{
			public int xMin, xMax, zMin, zMax;
		}

		List<MapRegion> regions;

		struct ClimateData
		{
			public float clouds, moisture;
		}

		List<ClimateData> climate = new List<ClimateData>();
		List<ClimateData> nextClimate = new List<ClimateData>();

		List<HexDirection> flowDirections = new List<HexDirection>();

		struct HexBiome
		{
			public int terrain, plant;

			public HexBiome(int terrain, int plant)
			{
				this.terrain = terrain;
				this.plant = plant;
			}
		}

		static float[] temperatureBands = { 0.1f, 0.3f, 0.6f };

		static float[] moistureBands = { 0.12f, 0.28f, 0.85f };

		static HexBiome[] biomes = {
		new HexBiome(0, 0), new HexBiome(4, 0), new HexBiome(4, 0), new HexBiome(4, 0),
		new HexBiome(0, 0), new HexBiome(2, 0), new HexBiome(2, 1), new HexBiome(2, 2),
		new HexBiome(0, 0), new HexBiome(1, 0), new HexBiome(1, 1), new HexBiome(1, 2),
		new HexBiome(0, 0), new HexBiome(1, 1), new HexBiome(1, 2), new HexBiome(1, 3)
	};

		public void GenerateMap(int x, int z)
		{
			Random.State originalRandomState = Random.state;
			if (!useFixedSeed)
			{
				seed = Random.Range(0, int.MaxValue);
				seed ^= (int)System.DateTime.Now.Ticks;
				seed ^= (int)Time.unscaledTime;
				seed &= int.MaxValue;
			}
			Random.InitState(seed);

			cellCount = x * z;
			grid.CreateMap(x, z);
			if (searchFrontier == null)
			{
				searchFrontier = new HexCellPriorityQueue();
			}
			for (int i = 0; i < cellCount; i++)
			{
				grid.GetCell(i).WaterLevel = waterLevel;
			}

			HexMetrics.globalWaterLevel = waterLevel;

			CreateRegions();
			CreateLand();
			ErodeLand();
			CreateClimate();
			//CreateRivers();
			SetTerrainType();

			//Some places in here we are changing heights which is fuckin shit up. we need to not do that. that or do it before rivers are created.
			GenerateHexCollections();
			CreateRivers();

			for (int i = 0; i < cellCount; i++)
			{
				grid.GetCell(i).SearchPhase = 0;
			}

			Random.state = originalRandomState;
		}

		void CreateRegions()
		{
			if (regions == null)
			{
				regions = new List<MapRegion>();
			}
			else
			{
				regions.Clear();
			}

			MapRegion region;
			switch (regionCount)
			{
				default:
					region.xMin = mapBorderX;
					region.xMax = grid.cellCountX - mapBorderX;
					region.zMin = mapBorderZ;
					region.zMax = grid.cellCountZ - mapBorderZ;
					regions.Add(region);
					break;
				case 2:
					if (Random.value < 0.5f)
					{
						region.xMin = mapBorderX;
						region.xMax = grid.cellCountX / 2 - regionBorder;
						region.zMin = mapBorderZ;
						region.zMax = grid.cellCountZ - mapBorderZ;
						regions.Add(region);
						region.xMin = grid.cellCountX / 2 + regionBorder;
						region.xMax = grid.cellCountX - mapBorderX;
						regions.Add(region);
					}
					else
					{
						region.xMin = mapBorderX;
						region.xMax = grid.cellCountX - mapBorderX;
						region.zMin = mapBorderZ;
						region.zMax = grid.cellCountZ / 2 - regionBorder;
						regions.Add(region);
						region.zMin = grid.cellCountZ / 2 + regionBorder;
						region.zMax = grid.cellCountZ - mapBorderZ;
						regions.Add(region);
					}
					break;
				case 3:
					region.xMin = mapBorderX;
					region.xMax = grid.cellCountX / 3 - regionBorder;
					region.zMin = mapBorderZ;
					region.zMax = grid.cellCountZ - mapBorderZ;
					regions.Add(region);
					region.xMin = grid.cellCountX / 3 + regionBorder;
					region.xMax = grid.cellCountX * 2 / 3 - regionBorder;
					regions.Add(region);
					region.xMin = grid.cellCountX * 2 / 3 + regionBorder;
					region.xMax = grid.cellCountX - mapBorderX;
					regions.Add(region);
					break;
				case 4:
					region.xMin = mapBorderX;
					region.xMax = grid.cellCountX / 2 - regionBorder;
					region.zMin = mapBorderZ;
					region.zMax = grid.cellCountZ / 2 - regionBorder;
					regions.Add(region);
					region.xMin = grid.cellCountX / 2 + regionBorder;
					region.xMax = grid.cellCountX - mapBorderX;
					regions.Add(region);
					region.zMin = grid.cellCountZ / 2 + regionBorder;
					region.zMax = grid.cellCountZ - mapBorderZ;
					regions.Add(region);
					region.xMin = mapBorderX;
					region.xMax = grid.cellCountX / 2 - regionBorder;
					regions.Add(region);
					break;
			}
		}

		void CreateLand()
		{
			int landBudget = Mathf.RoundToInt(cellCount * landPercentage * 0.01f);
			landCells = landBudget;
			for (int guard = 0; guard < 10000; guard++)
			{
				bool sink = Random.value < sinkProbability;
				for (int i = 0; i < regions.Count; i++)
				{
					MapRegion region = regions[i];
					int chunkSize = Random.Range(chunkSizeMin, chunkSizeMax - 1);
					if (sink)
					{
						landBudget = SinkTerrain(chunkSize, landBudget, region);
					}
					else
					{
						landBudget = RaiseTerrain(chunkSize, landBudget, region);
						if (landBudget == 0)
						{
							return;
						}
					}
				}
			}
			if (landBudget > 0)
			{
				OutputLogger.LogWarning("Failed to use up " + landBudget + " land budget.");
				landCells -= landBudget;
			}
		}

		int RaiseTerrain(int chunkSize, int budget, MapRegion region)
		{
			searchFrontierPhase += 1;
			HexCell firstCell = GetRandomCell(region);
			firstCell.SearchPhase = searchFrontierPhase;
			firstCell.Distance = 0;
			firstCell.SearchHeuristic = 0;
			searchFrontier.Enqueue(firstCell);
			HexCoordinates center = firstCell.coordinates;

			int rise = Random.value < highRiseProbability ? 2 : 1;
			int size = 0;
			while (size < chunkSize && searchFrontier.Count > 0)
			{
				HexCell current = searchFrontier.Dequeue();
				int originalElevation = current.Elevation;
				int newElevation = originalElevation + rise;
				if (newElevation > elevationMaximum)
				{
					continue;
				}
				current.Elevation = newElevation;
				if (
					originalElevation < waterLevel &&
					newElevation >= waterLevel && --budget == 0
				)
				{
					break;
				}
				size += 1;

				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
				{
					HexCell neighbor = current.GetNeighbor(d);
					if (neighbor && neighbor.SearchPhase < searchFrontierPhase)
					{
						neighbor.SearchPhase = searchFrontierPhase;
						neighbor.Distance = neighbor.coordinates.DistanceTo(center);
						neighbor.SearchHeuristic =
							Random.value < jitterProbability ? 1 : 0;
						searchFrontier.Enqueue(neighbor);
					}
				}
			}
			searchFrontier.Clear();
			return budget;
		}

		int SinkTerrain(int chunkSize, int budget, MapRegion region)
		{
			searchFrontierPhase += 1;
			HexCell firstCell = GetRandomCell(region);
			firstCell.SearchPhase = searchFrontierPhase;
			firstCell.Distance = 0;
			firstCell.SearchHeuristic = 0;
			searchFrontier.Enqueue(firstCell);
			HexCoordinates center = firstCell.coordinates;

			int sink = Random.value < highRiseProbability ? 2 : 1;
			int size = 0;
			while (size < chunkSize && searchFrontier.Count > 0)
			{
				HexCell current = searchFrontier.Dequeue();
				int originalElevation = current.Elevation;
				int newElevation = current.Elevation - sink;
				if (newElevation < elevationMinimum)
				{
					continue;
				}
				current.Elevation = newElevation;
				if (
					originalElevation >= waterLevel &&
					newElevation < waterLevel
				)
				{
					budget += 1;
				}
				size += 1;

				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
				{
					HexCell neighbor = current.GetNeighbor(d);
					if (neighbor && neighbor.SearchPhase < searchFrontierPhase)
					{
						neighbor.SearchPhase = searchFrontierPhase;
						neighbor.Distance = neighbor.coordinates.DistanceTo(center);
						neighbor.SearchHeuristic =
							Random.value < jitterProbability ? 1 : 0;
						searchFrontier.Enqueue(neighbor);
					}
				}
			}
			searchFrontier.Clear();
			return budget;
		}

		void ErodeLand()
		{
			List<HexCell> erodibleCells = ListPool<HexCell>.Get();
			for (int i = 0; i < cellCount; i++)
			{
				HexCell cell = grid.GetCell(i);
				if (IsErodible(cell))
				{
					erodibleCells.Add(cell);
				}
			}

			int targetErodibleCount =
				(int)(erodibleCells.Count * (100 - erosionPercentage) * 0.01f);

			while (erodibleCells.Count > targetErodibleCount)
			{
				int index = Random.Range(0, erodibleCells.Count);
				HexCell cell = erodibleCells[index];
				HexCell targetCell = GetErosionTarget(cell);

				cell.Elevation -= 1;
				targetCell.Elevation += 1;

				if (!IsErodible(cell))
				{
					erodibleCells[index] = erodibleCells[erodibleCells.Count - 1];
					erodibleCells.RemoveAt(erodibleCells.Count - 1);
				}

				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
				{
					HexCell neighbor = cell.GetNeighbor(d);
					if (
						neighbor && neighbor.Elevation == cell.Elevation + 2 &&
						!erodibleCells.Contains(neighbor)
					)
					{
						erodibleCells.Add(neighbor);
					}
				}

				if (IsErodible(targetCell) && !erodibleCells.Contains(targetCell))
				{
					erodibleCells.Add(targetCell);
				}

				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
				{
					HexCell neighbor = targetCell.GetNeighbor(d);
					if (
						neighbor && neighbor != cell &&
						neighbor.Elevation == targetCell.Elevation + 1 &&
						!IsErodible(neighbor)
					)
					{
						erodibleCells.Remove(neighbor);
					}
				}
			}

			ListPool<HexCell>.Add(erodibleCells);
		}

		bool IsErodible(HexCell cell)
		{
			int erodibleElevation = cell.Elevation - 2;
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				HexCell neighbor = cell.GetNeighbor(d);
				if (neighbor && neighbor.Elevation <= erodibleElevation)
				{
					return true;
				}
			}
			return false;
		}

		HexCell GetErosionTarget(HexCell cell)
		{
			List<HexCell> candidates = ListPool<HexCell>.Get();
			int erodibleElevation = cell.Elevation - 2;
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				HexCell neighbor = cell.GetNeighbor(d);
				if (neighbor && neighbor.Elevation <= erodibleElevation)
				{
					candidates.Add(neighbor);
				}
			}
			HexCell target = candidates[Random.Range(0, candidates.Count)];
			ListPool<HexCell>.Add(candidates);
			return target;
		}

		void CreateClimate()
		{
			climate.Clear();
			nextClimate.Clear();
			ClimateData initialData = new ClimateData();
			initialData.moisture = startingMoisture;
			ClimateData clearData = new ClimateData();
			for (int i = 0; i < cellCount; i++)
			{
				climate.Add(initialData);
				nextClimate.Add(clearData);
			}

			for (int cycle = 0; cycle < 40; cycle++)
			{
				for (int i = 0; i < cellCount; i++)
				{
					EvolveClimate(i);
				}
				List<ClimateData> swap = climate;
				climate = nextClimate;
				nextClimate = swap;
			}
		}

		void EvolveClimate(int cellIndex)
		{
			HexCell cell = grid.GetCell(cellIndex);
			ClimateData cellClimate = climate[cellIndex];

			if (cell.IsUnderwater)
			{
				cellClimate.moisture = 1f;
				cellClimate.clouds += evaporationFactor;
			}
			else
			{
				float evaporation = cellClimate.moisture * evaporationFactor;
				cellClimate.moisture -= evaporation;
				cellClimate.clouds += evaporation;
			}

			float precipitation = cellClimate.clouds * precipitationFactor;
			cellClimate.clouds -= precipitation;
			cellClimate.moisture += precipitation;

			float cloudMaximum = 1f - cell.ViewElevation / (elevationMaximum + 1f);
			if (cellClimate.clouds > cloudMaximum)
			{
				cellClimate.moisture += cellClimate.clouds - cloudMaximum;
				cellClimate.clouds = cloudMaximum;
			}

			HexDirection mainDispersalDirection = windDirection.Opposite();
			float cloudDispersal = cellClimate.clouds * (1f / (5f + windStrength));
			float runoff = cellClimate.moisture * runoffFactor * (1f / 6f);
			float seepage = cellClimate.moisture * seepageFactor * (1f / 6f);
			for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
			{
				HexCell neighbor = cell.GetNeighbor(d);
				if (!neighbor)
				{
					continue;
				}
				ClimateData neighborClimate = nextClimate[neighbor.Index];
				if (d == mainDispersalDirection)
				{
					neighborClimate.clouds += cloudDispersal * windStrength;
				}
				else
				{
					neighborClimate.clouds += cloudDispersal;
				}

				int elevationDelta = neighbor.ViewElevation - cell.ViewElevation;
				if (elevationDelta < 0)
				{
					cellClimate.moisture -= runoff;
					neighborClimate.moisture += runoff;
				}
				else if (elevationDelta == 0)
				{
					cellClimate.moisture -= seepage;
					neighborClimate.moisture += seepage;
				}

				nextClimate[neighbor.Index] = neighborClimate;
			}

			ClimateData nextCellClimate = nextClimate[cellIndex];
			nextCellClimate.moisture += cellClimate.moisture;
			if (nextCellClimate.moisture > 1f)
			{
				nextCellClimate.moisture = 1f;
			}
			nextClimate[cellIndex] = nextCellClimate;
			climate[cellIndex] = new ClimateData();
		}

		void CreateRivers()
		{
			List<HexCell> riverOrigins = ListPool<HexCell>.Get();
			for (int i = 0; i < cellCount; i++)
			{
				HexCell cell = grid.GetCell(i);
				if (cell.IsUnderwater)
				{
					continue;
				}
				ClimateData data = climate[i];
				float weight =
					data.moisture * (cell.Elevation - waterLevel) /
					(elevationMaximum - waterLevel);
				if (weight > 0.75f)
				{
					riverOrigins.Add(cell);
					riverOrigins.Add(cell);
				}
				if (weight > 0.5f)
				{
					riverOrigins.Add(cell);
				}
				if (weight > 0.25f)
				{
					riverOrigins.Add(cell);
				}
			}

			int riverBudget = Mathf.RoundToInt(landCells * riverPercentage * 0.01f);
			while (riverBudget > 0 && riverOrigins.Count > 0)
			{
				int index = Random.Range(0, riverOrigins.Count);
				int lastIndex = riverOrigins.Count - 1;
				HexCell origin = riverOrigins[index];
				riverOrigins[index] = riverOrigins[lastIndex];
				riverOrigins.RemoveAt(lastIndex);

				if (!origin.HasRiver)
				{
					bool isValidOrigin = true;
					for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
					{
						HexCell neighbor = origin.GetNeighbor(d);
						if (neighbor && (neighbor.HasRiver || neighbor.IsUnderwater))
						{
							isValidOrigin = false;
							break;
						}
					}
					if (isValidOrigin)
					{
						riverBudget -= CreateRiver(origin);
					}
				}
			}

			if (riverBudget > 0)
			{
				OutputLogger.LogWarning("Failed to use up river budget.");
			}

			ListPool<HexCell>.Add(riverOrigins);
		}

		int CreateRiver(HexCell origin)
		{
			int length = 1;
			HexCell cell = origin;
			HexDirection direction = HexDirection.NE;
			while (!cell.IsUnderwater)
			{
				int minNeighborElevation = int.MaxValue;
				flowDirections.Clear();
				for (HexDirection d = HexDirection.NE; d <= HexDirection.NW; d++)
				{
					HexCell neighbor = cell.GetNeighbor(d);
					if (!neighbor)
					{
						continue;
					}

					if (neighbor.Elevation < minNeighborElevation)
					{
						minNeighborElevation = neighbor.Elevation;
					}

					if (neighbor == origin || neighbor.HasIncomingRiver)
					{
						continue;
					}

					int delta = neighbor.Elevation - cell.Elevation;
					if (delta > 0)
					{
						continue;
					}

					if (neighbor.HasOutgoingRiver)
					{
						cell.SetOutgoingRiver(d);
						return length;
					}

					if (delta < 0)
					{
						flowDirections.Add(d);
						flowDirections.Add(d);
						flowDirections.Add(d);
					}
					if (
						length == 1 ||
						d != direction.Next2() && d != direction.Previous2()
					)
					{
						flowDirections.Add(d);
					}
					flowDirections.Add(d);
				}

				if (flowDirections.Count == 0)
				{
					if (length == 1)
					{
						return 0;
					}

					if (minNeighborElevation >= cell.Elevation)
					{
						cell.WaterLevel = minNeighborElevation;
						if (minNeighborElevation == cell.Elevation)
						{
							cell.Elevation = minNeighborElevation - 1;
						}
					}
					break;
				}

				direction = flowDirections[Random.Range(0, flowDirections.Count)];
				cell.SetOutgoingRiver(direction);
				length += 1;

				if (
					minNeighborElevation >= cell.Elevation &&
					Random.value < extraLakeProbability
				)
				{
					cell.WaterLevel = cell.Elevation;
					cell.Elevation -= 1;
				}

				//need to instead set a min length, store the chain of rivers in a list, wait until river complete, then set the ingoing/outgoings if possible
				cell = cell.GetNeighbor(direction);
			}
			return length;
		}

		void SetTerrainType()
		{
			temperatureJitterChannel = Random.Range(0, 4);
			int rockDesertElevation =
				elevationMaximum - (elevationMaximum - waterLevel) / 2;

			var averageTemp = 0.0f;

			for (int i = 0; i < cellCount; i++)
			{
				HexCell cell = grid.GetCell(i);
				float temperature = DetermineTemperature(cell);
				averageTemp += temperature;
				float moisture = climate[i].moisture;

				var biome = Biome.CalculateTerrainType(cell, temperature, moisture, elevationMaximum);
				cell.BiomeSubtype = biome;
			}

			OutputLogger.LogWarning($"Average Temperature: {averageTemp / cellCount}");
		}

		void GenerateHexCollections()
		{
			var firstStageCollections = new List<HexCollection>();
			var cellsCopy = new List<HexCell>(grid.cells);
			foreach(var type in System.Enum.GetValues(typeof(BiomeTerrainType)))
			{
				var terrainType = (BiomeTerrainType)type;
				var matchingCells = cellsCopy.Where(x => MatchTerrainTypes(terrainType, x.BiomeSubtype)).ToList();
				foreach(var c in matchingCells)
				{
					cellsCopy.Remove(c);
				}

				while(matchingCells.Count > 0)
				{
					var cell = matchingCells.First();
					matchingCells.Remove(cell);
					var collection = CreateNewHexCollection();
					collection.AffiliatedTerrainType = terrainType;
					CompileCollection(cell, ref matchingCells, ref collection);
					collection.cellCollection.OrderBy(x => x);
					for(int i = 0; i < collection.cellCollection.Count; i++)
					{
						var currentCell = grid.GetCell(collection.cellCollection[i]);
						currentCell.name = $"CellIndex: {cell.Index} || Chunk: {firstStageCollections.Count}/Cell: {i}";
						currentCell.hexCellLabel.name = $"Chunk: {firstStageCollections.Count}/Cell Label: {i}";
					}
					collection.Update(grid);
					firstStageCollections.Add(collection);
				}
			}

			var secondStageCollections = new List<HexCollection>();
			
			var smallBoys = firstStageCollections.Where(x => x.cellCollection.Count <= 5
			&& x.CollectionType != HexCollection.HexCollectionType.LAKE && x.CollectionType != HexCollection.HexCollectionType.ISLAND).ToList();
			while(smallBoys.Count > 0)
			{
				var found = false;
				var collection = smallBoys.First();
				var border = SimulationUtilities.FindBorderOutsideCells(collection.cellCollection);
				foreach (var cellIndex in border)
				{
					var borderCell = grid.GetCell(cellIndex);
					var borderCellHexCollection = firstStageCollections.First(x => x.cellCollection.Contains(cellIndex));
					if (borderCellHexCollection.IsUnderwater == collection.IsUnderwater)
					{
						borderCellHexCollection.cellCollection.AddRange(collection.cellCollection);
						firstStageCollections.Remove(collection);
						found = true;
						break;
					}
				}
				smallBoys.Remove(collection);
			}

			foreach (var collection in firstStageCollections)
			{
				//check for lakes
				if (collection.IsUnderwater && !SimulationUtilities.OutsideBorderContainsEdgeOfMap(collection.cellCollection))
				{
					var border = SimulationUtilities.FindBorderOutsideCells(collection.cellCollection);
					var landCount = 0;
					foreach (var borderCellIndex in border)
					{
						var borderCell = grid.GetCell(borderCellIndex);
						if (!borderCell.IsUnderwater)
						{
							landCount++;
						}
					}
					if (landCount == border.Count)
					{
						//set collection to be lake
						collection.CollectionType = HexCollection.HexCollectionType.LAKE;
					}
				}
				else if (!collection.IsUnderwater && collection.cellCollection.Count <= 5)
				{
					//check for islands
					var border = SimulationUtilities.FindBorderOutsideCells(collection.cellCollection);
					var underwaterCount = 0;
					foreach (var borderCellIndex in border)
					{
						var borderCell = grid.GetCell(borderCellIndex);
						if (borderCell.IsUnderwater)
						{
							underwaterCount++;
						}
					}
					if (underwaterCount == border.Count)
					{
						//set collection to be island
						collection.CollectionType = HexCollection.HexCollectionType.ISLAND;
					}
				}
			}

			secondStageCollections.AddRange(firstStageCollections);
			var underFives = secondStageCollections.Where(x => x.cellCollection.Count <= 5
			&& x.CollectionType != HexCollection.HexCollectionType.LAKE && x.CollectionType != HexCollection.HexCollectionType.ISLAND).ToList();
			OutputLogger.Log("Chunks that are still 5 or less: " + underFives.Count);

			foreach(var c in secondStageCollections)
			{
				c.Update(grid);
			}

			//third stage - group mountains together		
			var mountainousCollections = secondStageCollections.Where(x => x.CollectionType == HexCollection.HexCollectionType.MOUNTAINS).ToList();
			while(mountainousCollections.Count > 0)
			{
				var collection = mountainousCollections.First();
				var border = SimulationUtilities.FindBorderOutsideCells(collection.cellCollection);
				foreach (var cellIndex in border)
				{
					var borderCell = grid.GetCell(cellIndex);
					var borderCellHexCollections = firstStageCollections.Where(x => x.cellCollection.Contains(cellIndex)).ToList();
					if(borderCellHexCollections.Count == 0)
					{
						continue;
					}

					var borderCellHexCollection = borderCellHexCollections.First();
					if (borderCellHexCollection.CollectionType == collection.CollectionType)
					{
						borderCellHexCollection.cellCollection.AddRange(collection.cellCollection);
						secondStageCollections.Remove(collection);
						break;
					}
				}
				mountainousCollections.Remove(collection);
			}

			//now rivers
			//instead lets get all cells with only an outgoing river and just follow emb
			var riverCells = grid.cells.Where(x => x.HasOutgoingRiver && !x.HasIncomingRiver).ToList();

			while (riverCells.Count > 0)
			{
				var cell = riverCells.First();
				riverCells.Remove(cell);
				var collection = CreateNewHexCollection();
				collection.CollectionType = HexCollection.HexCollectionType.RIVER;
				//cant use compile, need a special one for rivers that follows just one river
				while(cell.HasOutgoingRiver)
				{
					collection.cellCollection.Add(cell.Index);
					cell = cell.GetNeighbor(cell.OutgoingRiver);
				}
				collection.cellCollection.Add(cell.Index);

				collection.cellCollection.OrderBy(x => x);
				collection.Update(grid);
				secondStageCollections.Add(collection);
			}

			foreach (var c in secondStageCollections)
			{
				c.Update(grid);
				c.Normalize(grid);
			}

			grid.CreateOverlay(secondStageCollections);

			foreach(var c in secondStageCollections)
			{
				EventManager.Instance.Dispatch(new AddContextEvent(c, true));
			}
		}

		HexCollection CreateNewHexCollection()
		{
			var hexCollection = new HexCollection();
			return hexCollection;
		}

		void CompileCollection(HexCell cell, ref List<HexCell> matchingCells, ref HexCollection hexCollection)
		{
			if (!hexCollection.cellCollection.Contains(cell.Index))
			{
				hexCollection.cellCollection.Add(cell.Index);
			}
			var matchingNeighbors = new List<HexCell>();

			for (Terrain.HexDirection d = Terrain.HexDirection.NE; d <= Terrain.HexDirection.NW; d++)
			{
				HexCell neighbor = cell.GetNeighbor(d);
				if (neighbor && matchingCells.Contains(neighbor))
				{
					matchingNeighbors.Add(neighbor);
				}
			}

			foreach(var neighbor in matchingNeighbors)
			{
				matchingCells.Remove(neighbor);
			}

			foreach(var neighbor in matchingNeighbors)
			{
				CompileCollection(neighbor, ref matchingCells, ref hexCollection);
			}
		}

		bool MatchTerrainTypes(BiomeTerrainType key, BiomeTerrainType value)
		{
			/*
			if(Biome.BiomeMatches.TryGetValue(key, out var list))
			{
				return list.Contains(value);
			}
			else
			{
				return false;
			}
			*/
			return key == value;
		}

		float DetermineTemperature(HexCell cell)
		{
			float latitude = (float)cell.coordinates.Z / grid.cellCountZ;
			if (hemisphere == HemisphereMode.Both)
			{
				latitude *= 2f;
				if (latitude > 1f)
				{
					latitude = 2f - latitude;
				}
			}
			else if (hemisphere == HemisphereMode.North)
			{
				latitude = 1f - latitude;
			}

			float temperature =
				Mathf.LerpUnclamped(lowTemperature, highTemperature, latitude);

			temperature *= 1f - (cell.ViewElevation - waterLevel) /
				(elevationMaximum - waterLevel + 1f);

			float jitter =
				HexMetrics.SampleNoise(cell.Position * 0.1f)[temperatureJitterChannel];

			temperature += (jitter * 2f - 1f) * temperatureJitter;

			return temperature;
		}

		HexCell GetRandomCell(MapRegion region)
		{
			return grid.GetCell(
				Random.Range(region.xMin, region.xMax),
				Random.Range(region.zMin, region.zMax)
			);
		}
	}
}