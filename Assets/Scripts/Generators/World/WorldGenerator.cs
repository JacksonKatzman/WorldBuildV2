using System.Collections.Generic;
using UnityEngine;
using Game.Generators.Noise;

public class WorldGenerator
{
	public static Texture2D GenerateWorldTexture()
	{
		Vector2Int noiseSize = new Vector2Int(100, 100);
		var noiseMap = NoiseGenerator.GeneratePerlinNoise(noiseSize, 26,
														 4, 0.456f,
														  2, new Vector2(0,0));
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

		return worldNoiseTexture;
	}

	public static float[,] GenerateWorldNoise()
	{
		Vector2Int noiseSize = new Vector2Int(15, 15);
		var noiseMap = NoiseGenerator.GeneratePerlinNoise(noiseSize, 26,
														 4, 0.456f,
														  2, new Vector2(0, 0));
		return noiseMap;
	}
}
