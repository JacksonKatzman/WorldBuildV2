using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.WorldGeneration;
using Game.Generators.Noise;

public class WorldGenerator
{
    public static World GenerateWorld(NoiseSettings noiseSettings, int chunkSize, List<Biome> biomes)
	{
		Vector2Int noiseSize = new Vector2Int(noiseSettings.worldSize.x * chunkSize, noiseSettings.worldSize.y * chunkSize);
		var noiseMap = NoiseGenerator.GeneratePerlinNoise(noiseSize, noiseSettings.scale,
														  noiseSettings.octaves, noiseSettings.persistance,
														  noiseSettings.lacunarity, noiseSettings.offset);
		var width = noiseMap.GetLength(0);
		var height = noiseMap.GetLength(1);

		Texture2D worldNoiseTexture = new Texture2D(width, height);
		Color[] colorMap = new Color[width * height];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
			}
		}

		worldNoiseTexture.SetPixels(colorMap);
		worldNoiseTexture.Apply();

		return new World(noiseMap, worldNoiseTexture, noiseSettings, chunkSize, biomes);
	}
}
