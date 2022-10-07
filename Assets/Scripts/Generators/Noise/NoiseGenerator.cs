using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Game.Generators.Noise;

public static class NoiseGenerator
{
	public static float[,] GeneratePerlinNoise(Vector2Int noiseDimensions, float scale, int octaves, float persistance, float lacunarity, Vector2 manualOffset)
	{
		Mathf.Clamp(scale, 0.001f, Mathf.Infinity);

		Vector2[] octaveOffsets = new Vector2[octaves];
		for(int i = 0; i < octaves; i++)
		{
			float offsetX = SimRandom.RandomInteger() + manualOffset.x;
			float offsetY = SimRandom.RandomInteger() + manualOffset.y;
			octaveOffsets[i] = new Vector2(offsetX, offsetY);
		}

		float[,] noiseMap = new float[noiseDimensions.x, noiseDimensions.y];

		float maxNoiseHeight = float.MinValue;
		float minNoiseHeight = float.MaxValue;

		float halfWidth = noiseDimensions.x / 2f;
		float halfHeight = noiseDimensions.y / 2f;

		for(int y = 0; y < noiseDimensions.y; y++)
		{
			for(int x = 0; x < noiseDimensions.x; x++)
			{
				float amplitude = 1;
				float frequency = 1;
				float noiseHeight = 0;

				for (int i = 0; i < octaves; i++)
				{
					float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
					float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

					float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
					noiseHeight += perlinValue * amplitude;

					amplitude *= persistance;
					frequency *= lacunarity;
				}

				maxNoiseHeight = Mathf.Max(noiseHeight, maxNoiseHeight);
				minNoiseHeight = Mathf.Min(noiseHeight, minNoiseHeight);

				noiseMap[x, y] = noiseHeight;
			}
		}

		for (int y = 0; y < noiseDimensions.y; y++)
		{
			for (int x = 0; x < noiseDimensions.x; x++)
			{
				noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
				//noiseMap[x, y] = Mathf.Round(noiseMap[x, y] * 10) / 10;
			}
		}
		return noiseMap;
	}

	public static Texture2D GenerateARGBNoiseTexture(NoiseSettings noiseSettings)
	{
		Texture2D worldNoiseTexture = new Texture2D(noiseSettings.worldSize.x, noiseSettings.worldSize.y, TextureFormat.RGBAFloat, true);

		/*
		Color[] colors = new Color[3];
		colors[0] = Color.red;
		colors[1] = Color.green;
		colors[2] = Color.blue;

		for (int mip = 0; mip < 3; mip++)
		{
			worldNoiseTexture.SetPixels(GenerateColorMap(noiseSettings, colors[mip]), mip);
		}
		*/

		worldNoiseTexture.SetPixels(Generate3DColorMap(noiseSettings));

		worldNoiseTexture.Apply();

/*
		for (int mip = 0; mip < 3; ++mip)
		{
			Color[] cols = GenerateColorMap(noiseSettings);
			for (int i = 0; i < cols.Length; ++i)
			{
				cols[i] = Color.Lerp(cols[i], colors[mip], 0.33f);
			}
			worldNoiseTexture.SetPixels(cols, mip);
		}

		worldNoiseTexture.Apply();
*/
		return worldNoiseTexture;
	}

	public static Color[] GenerateColorMap(NoiseSettings noiseSettings, Color color)
	{
		var noiseMap = GeneratePerlinNoise(noiseSettings.worldSize, noiseSettings.scale,
														  noiseSettings.octaves, noiseSettings.persistance,
														  noiseSettings.lacunarity, noiseSettings.offset);
		var width = noiseMap.GetLength(0);
		var height = noiseMap.GetLength(1);
		Color[] colorMap = new Color[width * height];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				var c = Color.Lerp(color, Color.white, noiseMap[x, y]);
				colorMap[y * width + x] = c;
			}
		}

		return colorMap;
	}

	public static Color[] Generate3DColorMap(NoiseSettings noiseSettings)
	{
		var noiseMaps = new List<float[,]>();
		for(int i = 0; i < 5; i++)
		{
			noiseMaps.Add(GeneratePerlinNoise(noiseSettings.worldSize, noiseSettings.scale,
														  noiseSettings.octaves, noiseSettings.persistance,
														  noiseSettings.lacunarity, noiseSettings.offset));
		}

		var width = noiseSettings.worldSize.x;
		var height = noiseSettings.worldSize.y;

		Color[] colorMap = new Color[width * height];
		for (int y = 0; y < height; y++)
		{
			for (int x = 0; x < width; x++)
			{
				//var c = Color.Lerp(color, Color.white, noiseMap[x, y]);
				var c = new Color(noiseMaps[0][x, y], noiseMaps[1][x, y], noiseMaps[2][x, y], noiseMaps[3][x, y]);
				c = Color.Lerp(c, Color.white, noiseMaps[4][x, y] * 0.66f);
				colorMap[y * width + x] = c;
			}
		}

		return colorMap;
	}

}
