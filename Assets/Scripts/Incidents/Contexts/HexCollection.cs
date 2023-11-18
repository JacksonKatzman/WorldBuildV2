using Game.Simulation;
using Game.Terrain;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class HexCollection : InertIncidentContext, ITerrainTypeAffiliated, ILocationAffiliated
	{
		private static Dictionary<BiomeTerrainType, string> genericBiomeNames = new Dictionary<BiomeTerrainType, string>()
		{
			{BiomeTerrainType.Badlands, "arid wastes" }, {BiomeTerrainType.Deep_Ocean, "ocean"}, {BiomeTerrainType.Desert, "desert"},
			{BiomeTerrainType.Forest, "forests" }, {BiomeTerrainType.Grassland, "fields"}, {BiomeTerrainType.Ocean, "sea"},
			{BiomeTerrainType.Polar, "frostlands" }, {BiomeTerrainType.Rainforest, "jungle"}, {BiomeTerrainType.Reef, "sea"},
			{BiomeTerrainType.Shrubland, "arid fields" }, {BiomeTerrainType.Swamp, "marshes"}, {BiomeTerrainType.Taiga, "forested mountains"},
			{BiomeTerrainType.Tundra, "tundra" }
		};

		public enum HexCollectionType { UNKNOWN, LAKE, ISLAND }

		public override string Name
		{
			get
			{
				if(string.IsNullOrEmpty(name))
				{
					var closestCity = SimulationUtilities.GetCityNearestLocation(CurrentLocation);
					var biomeString = genericBiomeNames[AffiliatedTerrainType];
					if(IsMountainous)
					{
						biomeString = "mountains";
					}
					return $"the {biomeString} near {closestCity.Name}";
				}
				else
				{
					return name;
				}
			}
			set
			{
				name = value;
			}
		}
		public List<int> cellCollection;
		public BiomeTerrainType AffiliatedTerrainType { get; set; }
		public HexCollectionType CollectionType { get; set; }
		public bool IsMountainous { get; set; }
		public bool IsUnderwater { get; set; }
		public float AverageElevation => GetAverageElevation();

		public Location CurrentLocation { get; private set; }
		public HexGridChunk HexGridChunk { get; set; }
		private string name;

		public HexCollection()
		{
			cellCollection = new List<int>();
		}

		public HexCollection(int firstId)
		{
			cellCollection = new List<int>();
			cellCollection.Add(firstId);
		}

		public void Update(HexGrid grid)
		{
			var hasMountains = 0;
			var hasWater = 0;
			foreach (var cellIndex in cellCollection)
			{
				var cell = grid.GetCell(cellIndex);
				if (cell.IsMountainous)
				{
					hasMountains++;
				}

				if (cell.IsUnderwater)
				{
					hasWater++;
				}
			}

			IsMountainous = hasMountains >= (cellCollection.Count / 2) + 1 ? true : false;
			IsUnderwater = hasWater == cellCollection.Count ? true : false;

			var updatedCenter = GetCenter();
			if(CurrentLocation == null)
			{
				CurrentLocation = new Location(updatedCenter);
				EventManager.Instance.Dispatch(new AddContextEvent(CurrentLocation, true));
			}
			else
			{
				CurrentLocation.TileIndex = updatedCenter;
			}
		}

		public void Normalize(HexGrid grid)
		{
			var frequency = new Dictionary<BiomeTerrainType, float>();
			foreach (var cellIndex in cellCollection)
			{
				var cell = grid.GetCell(cellIndex);
				if(cell.Elevation > AverageElevation)
				{
					cell.Elevation = ((int)AverageElevation);
				}
				var terrainType = cell.TerrainType;
				if (!frequency.ContainsKey(terrainType))
				{
					frequency.Add(terrainType, 0);
				}
				frequency[terrainType]++;
			}

			var mostCommonTerrainType = frequency.OrderByDescending(x => x.Value).First().Key;
			foreach(var cellIndex in cellCollection)
			{
				var cell = grid.GetCell(cellIndex);
				if(mostCommonTerrainType == BiomeTerrainType.Desert)
				{
					cell.TerrainType = BiomeTerrainType.Desert;
				}
			}

			AffiliatedTerrainType = mostCommonTerrainType;
			/*
				var mostCommonTerrainType = frequency.OrderByDescending(x => x.Value).First().Key;
				var total = 0f;
				foreach(var pair in frequency)
				{
					total += pair.Value;
				}
				var frequencyPercentages = new Dictionary<BiomeTerrainType, float>();
				foreach (var pair in frequency)
				{
					frequencyPercentages.Add(pair.Key, pair.Value / total);
				}

				foreach (var cellIndex in cellCollection)
				{
					var cell = grid.GetCell(cellIndex);
					if(frequencyPercentages[cell.TerrainType] <= 0.02f)
					{
						cell.TerrainType = mostCommonTerrainType;
					}
				}
				*/
		}

		private int GetCenter()
		{
			//temporary
			var centroid = SimulationUtilities.GetCentroid(cellCollection);
			HexCell closest = null;
			var closestDistance = int.MaxValue;
			var grid = World.CurrentWorld.HexGrid;

			foreach(var cellIndex in cellCollection)
			{
				var cell = grid.GetCell(cellIndex);
				var dist = centroid.coordinates.DistanceTo(cell.coordinates);
				if(dist < closestDistance)
				{
					closest = cell;
					closestDistance = dist;
				}
			}

			return closest.Index;
		}

		private float GetAverageElevation()
		{
			return (float)cellCollection.Average();
		}
	}
}