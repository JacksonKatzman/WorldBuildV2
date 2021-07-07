using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
			}
		}
		return noiseMap;
	}

}
