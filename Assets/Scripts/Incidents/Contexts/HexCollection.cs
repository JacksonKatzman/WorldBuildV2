﻿using Game.Simulation;
using Game.Terrain;
using System.Collections.Generic;

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
		public bool IsMountainous { get; set; }

		public Location CurrentLocation { get; private set; }
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

		public void Initialize(HexGrid grid)
		{
			var hasMountains = 0;
			foreach(var cellIndex in cellCollection)
			{
				if(grid.GetCell(cellIndex).Elevation > 3)
				{
					hasMountains++;
				}
			}

			IsMountainous = hasMountains >= (cellCollection.Count / 2) + 1 ? true : false;
			CurrentLocation = GetCenter();
		}

		private Location GetCenter()
		{
			//temporary
			var location = new Location(cellCollection[0]);
			EventManager.Instance.Dispatch(new AddContextEvent(location, true));
			return location;
		}
	}
}