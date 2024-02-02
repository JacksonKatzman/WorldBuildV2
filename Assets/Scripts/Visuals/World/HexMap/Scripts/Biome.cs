using Game.Debug;
using Game.Incidents;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Terrain
{
    //Taiga, Tundra, Desert, Tropical Rainforest, Temperate Forest, Polar
	//public enum TerrainSupertype { Swamp, Grassland, Desert, Tundra, Badlands, Ocean };
    public enum BiomeTerrainType { Swamp, Grassland, Forest, Rainforest, Desert, Taiga, Tundra, Shrubland, Badlands, Polar, Reef, Ocean, Deep_Ocean };
	public class Biome
	{
		/*
		static float[] temperatureBands = { 0.1f, 0.3f, 0.6f, 10.0f };

		static float[] moistureBands = { 0.12f, 0.28f, 0.85f, 10.0f };

		static float[] elevationBands = { 0.1f, 0.25f, 0.55f, 10.0f };

		public static Dictionary<BiomeTerrainType, Vector2Int> BiomeInfo = new Dictionary<BiomeTerrainType, Vector2Int>
		{
			{BiomeTerrainType.Swamp, new Vector2Int(0,2)},
			{BiomeTerrainType.Grassland, new Vector2Int(1,1)},
			{BiomeTerrainType.Forest, new Vector2Int(2,3)},
			{BiomeTerrainType.Rainforest, new Vector2Int(3,3)},
			{BiomeTerrainType.Desert, new Vector2Int(4,0)},
			{BiomeTerrainType.Taiga, new Vector2Int(5,2)},
			{BiomeTerrainType.Tundra, new Vector2Int(6,1)},
			{BiomeTerrainType.Shrubland, new Vector2Int(7,1)},
			{BiomeTerrainType.Badlands, new Vector2Int(8,0)},
			{BiomeTerrainType.Polar, new Vector2Int(9,0)},
			{BiomeTerrainType.Reef, new Vector2Int(2,2)},
			{BiomeTerrainType.Ocean, new Vector2Int(4,0)},
			{BiomeTerrainType.Deep_Ocean, new Vector2Int(1,0)}
		};

		public static Dictionary<BiomeTerrainType, List<BiomeTerrainType>> BiomeMatches = new Dictionary<BiomeTerrainType, List<BiomeTerrainType>>
		{
			{BiomeTerrainType.Swamp, new List<BiomeTerrainType>() { BiomeTerrainType.Swamp } },
			{BiomeTerrainType.Grassland, new List<BiomeTerrainType>() { BiomeTerrainType.Grassland, BiomeTerrainType.Shrubland, BiomeTerrainType.Tundra } },
			{BiomeTerrainType.Forest, new List<BiomeTerrainType>() { BiomeTerrainType.Forest, BiomeTerrainType.Rainforest, BiomeTerrainType.Taiga } },
			{BiomeTerrainType.Rainforest, new List<BiomeTerrainType>() { BiomeTerrainType.Rainforest, BiomeTerrainType.Forest } },
			{BiomeTerrainType.Desert, new List<BiomeTerrainType>() { BiomeTerrainType.Desert, BiomeTerrainType.Shrubland, BiomeTerrainType.Badlands } },
			{BiomeTerrainType.Taiga, new List<BiomeTerrainType>() { BiomeTerrainType.Taiga, BiomeTerrainType.Polar } },
			{BiomeTerrainType.Tundra, new List<BiomeTerrainType>() { BiomeTerrainType.Tundra, BiomeTerrainType.Shrubland, BiomeTerrainType.Polar } },
			{BiomeTerrainType.Shrubland, new List<BiomeTerrainType>() { BiomeTerrainType.Shrubland, BiomeTerrainType.Grassland } },
			{BiomeTerrainType.Badlands, new List<BiomeTerrainType>() { BiomeTerrainType.Badlands, BiomeTerrainType.Desert } },
			{BiomeTerrainType.Polar, new List<BiomeTerrainType>() { BiomeTerrainType.Polar, BiomeTerrainType.Tundra, BiomeTerrainType.Taiga } },
			{BiomeTerrainType.Reef, new List<BiomeTerrainType>() { BiomeTerrainType.Reef, BiomeTerrainType.Ocean, BiomeTerrainType.Deep_Ocean } },
			{BiomeTerrainType.Ocean, new List<BiomeTerrainType>() { BiomeTerrainType.Ocean, BiomeTerrainType.Reef, BiomeTerrainType.Deep_Ocean } },
			{BiomeTerrainType.Deep_Ocean, new List<BiomeTerrainType>() { BiomeTerrainType.Deep_Ocean, BiomeTerrainType.Reef, BiomeTerrainType.Ocean } }
		};

		public static Dictionary<int, List<BiomeTerrainType>> BiomesByElevation = new Dictionary<int, List<BiomeTerrainType>>
		{
			{ 0, new List<BiomeTerrainType> {
			BiomeTerrainType.Tundra, BiomeTerrainType.Tundra, BiomeTerrainType.Polar, BiomeTerrainType.Polar,
			BiomeTerrainType.Polar, BiomeTerrainType.Tundra, BiomeTerrainType.Tundra, BiomeTerrainType.Polar,
			BiomeTerrainType.Desert, BiomeTerrainType.Shrubland, BiomeTerrainType.Grassland, BiomeTerrainType.Swamp,
			BiomeTerrainType.Desert, BiomeTerrainType.Desert, BiomeTerrainType.Grassland, BiomeTerrainType.Swamp } },
			{ 1, new List<BiomeTerrainType> {
			BiomeTerrainType.Tundra, BiomeTerrainType.Tundra, BiomeTerrainType.Polar, BiomeTerrainType.Polar,
			BiomeTerrainType.Polar, BiomeTerrainType.Tundra, BiomeTerrainType.Tundra, BiomeTerrainType.Polar,
			BiomeTerrainType.Shrubland, BiomeTerrainType.Grassland, BiomeTerrainType.Grassland, BiomeTerrainType.Forest,
			BiomeTerrainType.Desert, BiomeTerrainType.Shrubland, BiomeTerrainType.Grassland, BiomeTerrainType.Rainforest} },
			{ 2, new List<BiomeTerrainType> {
			BiomeTerrainType.Polar, BiomeTerrainType.Polar, BiomeTerrainType.Taiga, BiomeTerrainType.Taiga,
			BiomeTerrainType.Polar, BiomeTerrainType.Badlands, BiomeTerrainType.Taiga, BiomeTerrainType.Taiga,
			BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Forest, BiomeTerrainType.Forest,
			BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Rainforest, BiomeTerrainType.Rainforest} },
			{ 3, new List<BiomeTerrainType> { 
			BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Taiga, BiomeTerrainType.Taiga,
			BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Taiga, BiomeTerrainType.Taiga,
			BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Forest, BiomeTerrainType.Forest,
			BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Badlands, BiomeTerrainType.Badlands} }
		};
		*/

		public static BiomeTerrainType CalculateTerrainType(HexCell cell, float temperature, float moistureLevel, int elevationMaximum)
		{
			if (!cell.IsUnderwater)
            {
				//var inRange = AssetService.Instance.BiomeDataContainer.biomeData.Where(x => temperature >= x.minTemperature
				//&& temperature <= x.maxTemperature && moistureLevel >= x.minMoisture && moistureLevel <= x.maxMoisture && elevationMaximum >= x.minHeight && elevationMaximum <= x.maxHeight);
				var container = AssetService.Instance.BiomeDataContainer;
				/*
				var heightMatches = container.biomeData.Where(x => elevationMaximum >= x.minHeight && elevationMaximum <= x.maxHeight)
					.Aggregate((i1, i2) => (i1.maxHeight - elevationMaximum) > (i2.maxHeight - elevationMaximum) ? i1 : i2);
				*/
				if(cell.HasRiver)
                {
					moistureLevel += 0.25f;
                }

				//LABEL HEIGHT
				//cell.SetLabel((cell.Elevation - HexMetrics.globalWaterLevel).ToString());

				moistureLevel = Mathf.Clamp01(moistureLevel);
				var elevation = cell.Elevation - HexMetrics.globalWaterLevel;

				//really i just need to simplify - cut out unnecessary biomes down to like 5 and make sure the numbers line up, then worry about veg
				var heightMatches = container.biomeData.Where(x => elevation >= x.minHeight && elevation <= x.maxHeight);
				if(heightMatches.Count() == 0)
                {
					heightMatches = container.biomeData;
					OutputLogger.LogWarning("No height matches found when calculating biome.");
                }
				var moistureMatches = heightMatches.Where(x => moistureLevel >= x.minMoisture && moistureLevel <= x.maxMoisture);
				if(moistureMatches.Count() == 0)
                {
					moistureMatches = heightMatches;
					OutputLogger.LogWarning($"No moisture matches found when calculating biome. H: {elevation}, M: {moistureLevel}, T: {temperature}");
				}
				//var temperatureMatch = moistureMatches.Aggregate((i1, i2) => Mathf.Abs(i1.maxTemperature - temperature) < Mathf.Abs(i2.maxTemperature - temperature) ? i1 : i2);
				var currentMatch = moistureMatches.First();
				var difference = float.MaxValue;
				foreach(var match in moistureMatches)
                {
					var check = Mathf.Abs(temperature - match.maxTemperature);
					if(check < difference && temperature <= match.maxTemperature)
                    {
						difference = check;
						currentMatch = match;
                    }
                }

				//LABEL BIOME
				cell.SetLabel(currentMatch.terrainType.ToString());
				return currentMatch.terrainType;

				/*
				int m = 0;
				for (; m < moistureBands.Length; m++)
				{
					if (moistureLevel < moistureBands[m])
					{
						break;
					}
				}

				cell.PlantLevel = m;
				*/

				//return temperatureMatch != null ? temperatureMatch.terrainType : BiomeTerrainType.Grassland;
			}
			else
            {
				return BiomeTerrainType.Reef;
            }
        }

	}
}