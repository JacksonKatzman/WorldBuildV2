using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;

namespace Game.WorldGeneration
{
	public class Chunk : ITimeSensitive, IMutableZone
	{
		private World world;
		public Vector2Int coords;
		public Tile[,] chunkTiles;
		public float[,] noiseMap;

		public Chunk(World world, Vector2Int coords)
		{
			this.world = world;
			this.coords = coords;

			noiseMap = world.SampleNoiseMap(MapCategory.TERRAIN, coords);
			SetInitialValues();
			BuildTiles();
		}

		public void AdvanceTime()
		{
			foreach(Tile tile in chunkTiles)
			{
				tile.AdvanceTime();
			}
		}

		public float SampleNoiseMap(MapCategory mapCategory, Vector2Int tileLocation)
		{
			var sampleWorldMap = world.SampleNoiseMap(mapCategory, coords);
			return sampleWorldMap[tileLocation.x, tileLocation.y];
		}

		public Color[,] GetBiomeColorMap()
		{
			var width = chunkTiles.GetLength(0);
			var height = chunkTiles.GetLength(1);
			Color[,] colorMap = new Color[width, height];

			for(int y = 0; y < height; y++)
			{
				for(int x = 0; x < width; x++)
				{
					colorMap[x, y] = Color.red;
				}
			}

			return colorMap;
		}

		private void SetInitialValues()
		{

		}

		private void BuildTiles()
		{
			var width = noiseMap.GetLength(0);
			var height = noiseMap.GetLength(1);
			chunkTiles = new Tile[width, height];

			for (int y = 0; y < height; y++)
			{
				for (int x = 0; x < width; x++)
				{
					chunkTiles[x, y] = new Tile(world, this, new Vector2Int(x, y));
				}
			}
		}

		
	}
}
