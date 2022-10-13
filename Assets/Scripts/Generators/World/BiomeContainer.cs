using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.WorldGeneration;
using System.Linq;
using Game.Enums;

[CreateAssetMenu(fileName = nameof(BiomeContainer), menuName = "ScriptableObjects/Biomes/" + nameof(BiomeContainer), order = 2)]
public class BiomeContainer : ScriptableObject
{
	private static float MOUNTAIN_THRESHOLD = 0.8f;
	private static float HILL_THRESHOLD = 0.6f;
	private static float FLAT_THRESHOLD = 0.15f;

	private static float HOT_THRESHOLD = 0.8f;
	private static float TEMPERATE_THRESHOLD = 0.21f;

	public List<OldBiome> biomes;

	public OldBiome DetermineBiome(float height, float rainfall, float fertility, float temperature)
	{
		var bestScore = float.MaxValue;
		OldBiome bestFit = biomes[0];

		LandType landType = LandType.FLAT;
		LandTemperature landTemperature = LandTemperature.COLD;

		if(height >= MOUNTAIN_THRESHOLD)
		{
			landType = LandType.MOUNTAINS;
		}
		else if(height >= HILL_THRESHOLD)
		{
			landType = LandType.HILLS;
		}
		else if(height >= FLAT_THRESHOLD)
		{
			landType = LandType.FLAT;
		}

		if(temperature >= HOT_THRESHOLD)
		{
			landTemperature = LandTemperature.HOT;
		}
		else if(temperature >= TEMPERATE_THRESHOLD)
		{
			landTemperature = LandTemperature.TEMPERATE;
		}

		var possibleBiomes = biomes.Where(x => x.landType == landType && x.temperature == landTemperature);

		foreach(OldBiome biome in possibleBiomes)
		{
			var rainfallScore = Mathf.Abs(biome.rainfallThreshold - rainfall);
			var fertilityScore = Mathf.Abs(biome.fertilityThreshold - fertility);

			var totalScore = fertilityScore + rainfallScore;

			if (totalScore < bestScore)
			{
				bestScore = totalScore;
				bestFit = biome;
			}
		}

		return bestFit;
	}
}