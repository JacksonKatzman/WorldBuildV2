using Game.Simulation;
using Game.Terrain;
using Game.Utilities;
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

		public enum HexCollectionType { UNKNOWN, LAKE, ISLAND, MOUNTAINS, RIVER }

		public override string Name
		{
			get
			{
				if(string.IsNullOrEmpty(name))
				{
					var closestCity = SimulationUtilities.GetCityNearestLocation(CurrentLocation);
					var biomeString = genericBiomeNames[AffiliatedTerrainType];
					if(CollectionType == HexCollectionType.MOUNTAINS)
					{
						biomeString = "mountains";
					}
					else if(CollectionType == HexCollectionType.RIVER)
					{
						biomeString = "river";
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
		//public bool IsMountainous { get; set; }
		public bool IsUnderwater { get; set; }
		public float AverageElevation => GetAverageElevation();

		public Location CurrentLocation { get; private set; }
		public HexGridOverlayChunk OverlayChunk { get; set; }

        public override string Description => $"{GetTerrainTypeString()} near {SimulationUtilities.GetCityNearestLocation(CurrentLocation).Link()}";

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

			var isMountainous = hasMountains >= (cellCollection.Count / 2) + 1 ? true : false;
			if(isMountainous && CollectionType != HexCollectionType.RIVER)
			{
				CollectionType = HexCollectionType.MOUNTAINS;
			}

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

		public void ChangeBiomeSubtype(HexGrid grid, BiomeTerrainType subtype)
        {
			var cells = grid.GetHexCells(cellCollection);
			foreach(var cell in cells)
            {
				cell.BiomeSubtype = subtype;
				cell.chunk.Refresh();
            }
        }

		public void Normalize(HexGrid grid, bool updateHeight = false)
		{
			var frequency = new Dictionary<BiomeTerrainType, float>();
			var averageElevation = AverageElevation;
			foreach (var cellIndex in cellCollection)
			{
				var cell = grid.GetCell(cellIndex);
				if(updateHeight && cell.Elevation > averageElevation)
				{
					if (SimRandom.RandomFloat01() > 0.6f)
					{
						cell.Elevation = ((int)averageElevation);
					}
				}
				var terrainType = cell.BiomeSubtype;
				if (!frequency.ContainsKey(terrainType))
				{
					frequency.Add(terrainType, 0);
				}
				frequency[terrainType]++;
			}

			/*
			var mostCommonTerrainType = frequency.OrderByDescending(x => x.Value).First().Key;
			foreach(var cellIndex in cellCollection)
			{
				var cell = grid.GetCell(cellIndex);
				if(mostCommonTerrainType == BiomeTerrainType.Desert)
				{
					cell.BiomeSubtype = BiomeTerrainType.Desert;
					cell.chunk.Refresh();
				}
			}
			*/

			//AffiliatedTerrainType = mostCommonTerrainType;
			
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
				if(frequencyPercentages[cell.BiomeSubtype] <= 0.02f)
				{
					cell.BiomeSubtype = mostCommonTerrainType;
					cell.chunk.Refresh();
				}
			}
			AffiliatedTerrainType = mostCommonTerrainType;
		}

		private string GetTerrainTypeString()
        {
			return string.Join(" ", genericBiomeNames[AffiliatedTerrainType].Split(' ').Select(word => char.ToUpper(word[0]) + word.Substring(1)));
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
			//return (float)cellCollection.Average();
			var grid = World.CurrentWorld.HexGrid;
			var heights = grid.GetHexCells(cellCollection).Select(x => x.Elevation);
			return (float)heights.Average();
		}

		public int GetMaxElevation()
        {
			var grid = World.CurrentWorld.HexGrid;
			var heights = grid.GetHexCells(cellCollection).Select(x => x.Elevation);
			return (int)heights.Max();
		}
	}
}