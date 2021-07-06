using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;

namespace Game.WorldGeneration
{
	[System.Serializable]
	public class Biome
	{
		private static float OCEAN_THRESHOLD = 0.2f;
		private static float FLAT_THRESHOLD = 0.5f;
		private static float HILLS_THRESHOLD = 0.7f;
		private static float MOUNTAINS_THRESHOLD = 1.1f;

		public string biomeType;

		public LandType landType;
		[Range(0.0f, 1.0f)]
		public float rainfallThreshold;
		[Range(0.0f, 1.0f)]
		public float fertilityThreshold;

		public Color color;

		[Range(0.0f, 1.0f)]
		public float availableLand;

		public static LandType CalculateLandType(float elevation)
		{
			LandType type = LandType.MOUNTAINS;
			if(elevation < OCEAN_THRESHOLD)
			{
				type = LandType.OCEAN;
			}
			else if(elevation < FLAT_THRESHOLD)
			{
				type = LandType.FLAT;
			}
			else if(elevation < HILLS_THRESHOLD)
			{
				type = LandType.HILLS;
			}

			return type;
		}

		public static Biome CalculateBiomeType(List<Biome> biomes, LandType landType, float rainfall, float fertility)
		{
			if(landType == LandType.HILLS)
			{
				landType = LandType.FLAT;
			}
			float currentBestScore = float.MaxValue;
			Biome currentBestBiome = null;

			foreach(Biome biome in biomes)
			{
				if(landType != biome.landType)
				{
					continue;
				}

				var rainfallScore = Mathf.Abs(rainfall - biome.rainfallThreshold);
				var fertilityScore = Mathf.Abs(fertility - biome.fertilityThreshold);

				var finalScore = rainfall + fertilityScore;
				if(finalScore < currentBestScore)
				{
					currentBestScore = finalScore;
					currentBestBiome = biome;
				}
			}

			if(currentBestBiome == null)
			{
				currentBestBiome = biomes[0];
			}

			return currentBestBiome;
		}
	}
}
