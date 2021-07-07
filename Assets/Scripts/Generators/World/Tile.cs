using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;
using Game.Factions;

namespace Game.WorldGeneration
{
    public class Tile : ITimeSensitive, IMutableZone
    {
        public World world;
        public Chunk chunk;
        public Vector2Int coords;

		public Biome biome;
		public LandType landType;
		public float baseMoisture;
		public float baseFertility;

		public List<Landmark> landmarks;

		public Tile(World world, Chunk chunk, Vector2Int coords)
		{
			this.world = world;
			this.chunk = chunk;
			this.coords = coords;

			landmarks = new List<Landmark>();

			CalculateBiome();
		}

		public City SpawnCity(Faction faction, float food, int population)
		{
			var city = new City(this, faction, food, population);
			landmarks.Add(city);
			return city;
		}

		private float Value => chunk.noiseMap[coords.x, coords.y];
		public float rainfallValue => chunk.SampleNoiseMap(MapCategory.RAINFALL, coords);
		public void AdvanceTime()
		{
			foreach(Landmark landmark in landmarks)
			{
				if(landmark is City)
				{

				}
			}
		}

		public Vector2Int GetWorldPosition()
		{
			return new Vector2Int(world.chunkSize * chunk.coords.x + coords.x, world.chunkSize * chunk.coords.y + coords.y);
		}

		public List<Tile> GetAllTilesInRadius(int radius)
		{
			var worldCoords = GetWorldPosition();
			var map = world.noiseMaps[MapCategory.TERRAIN];
			var tileList = new List<Tile>();

			for (int y = worldCoords.y - radius; y < worldCoords.y + radius; y++)
			{
				for(int x = worldCoords.x - radius; x < worldCoords.x + radius; x++)
				{
					if(x < map.GetLength(0) && y < map.GetLength(1))
					{
						bool isInCircle = Mathf.Pow((x - worldCoords.x), 2) + Mathf.Pow((y - worldCoords.y), 2) < Mathf.Pow(radius, 2);
						if(isInCircle)
						{
							tileList.Add(world.GetTileAtWorldPosition(new Vector2Int(x, y)));
						}
					}
				}
			}
			return tileList;
		}

		private void CalculateBiome()
		{
			landType = Biome.CalculateLandType(Value);
			baseMoisture = chunk.SampleNoiseMap(MapCategory.RAINFALL, coords);
			baseFertility = chunk.SampleNoiseMap(MapCategory.FERTILITY, coords);

			baseFertility += (baseMoisture / 10.0f);
			baseFertility -= (Value / 10.0f);

			biome = world.CalculateTileBiome(landType, baseMoisture, baseFertility);
		}
    }
}
