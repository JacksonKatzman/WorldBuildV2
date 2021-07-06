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

		public bool SpawnCity(float food, int population)
		{
			bool spawned = false;
			int attempts = 0;
			while (!spawned && attempts < 10)
			{
				var randomXIndex = WorldHandler.Instance.RandomRange(0, chunkTiles.GetLength(0));
				var randomYIndex = WorldHandler.Instance.RandomRange(0, chunkTiles.GetLength(1));
				var chosenTile = chunkTiles[randomXIndex, randomYIndex];
				var uncontrolled = (world.GetFactionThatControlsTile(chosenTile) == null);
				if(chosenTile.baseFertility > 0.5f && chosenTile.landmarks.Count == 0 && uncontrolled)
				{
					//chosenTile.SpawnCity(food, population);
					world.CreateNewFaction(chosenTile, food, population);
					spawned = true;
					OutputLogger.LogFormatAndPause("Spawned city in chunk ({0},{1}) in tile ({2},{3})).", LogSource.WORLDGEN, coords.x, coords.y, randomXIndex, randomYIndex);
				}
				attempts++;
			}
			if(attempts >= 10)
			{
				//Debug.LogFormat("Failed to spawn city in chunk ({0},{1})", coords.x, coords.y);
			}
			return spawned;
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
					colorMap[x, y] = chunkTiles[x, y].biome.color;
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
