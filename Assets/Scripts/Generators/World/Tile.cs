using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;
using Game.Factions;
using Game.Data.EventHandling.EventRecording;

namespace Game.WorldGeneration
{
    public class Tile : ITimeSensitive, IMutableZone, IRecordable
    {
		public string Name => name;
        public World world;
        public Chunk chunk;
        public Vector2Int coords;
		public Faction controller;

		public Biome biome;
		public LandType landType;
		public float baseMoisture;
		public float baseFertility;

		public readonly List<Direction> riverDirections;
		public readonly List<Direction> roadDirections;

		public List<Landmark> landmarks;

		private string name;

		public Tile(World world, Chunk chunk, Vector2Int coords)
		{
			this.world = world;
			this.chunk = chunk;
			this.coords = coords;

			riverDirections = new List<Direction>();
			roadDirections = new List<Direction>();

			landmarks = new List<Landmark>();

			CalculateBiome();
		}

		public void AddRiverDirection(Direction d)
		{
			if(!riverDirections.Contains(d))
			{
				riverDirections.Add(d);
			}
		}

		public void AddRoadDirection(Direction d)
		{
			if (!roadDirections.Contains(d))
			{
				roadDirections.Add(d);
			}
		}

		private float Value => chunk.noiseMap[coords.x, coords.y];
		public float rainfallValue => chunk.SampleNoiseMap(MapCategory.RAINFALL, coords);
		public void AdvanceTime()
		{
			foreach (Landmark landmark in landmarks)
			{
				landmark.AdvanceTime();
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
					if(x < map.GetLength(0) && y < map.GetLength(1) && x > 0 && y > 0)
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

		public List<Tile> GetAllTilesInRing(int outerRadius, int innerRadius)
		{
			var outer = GetAllTilesInRadius(outerRadius);
			var inner = GetAllTilesInRadius(innerRadius);
			foreach(Tile tile in inner)
			{
				if(outer.Contains(tile))
				{
					outer.Remove(tile);
				}
			}
			return outer;
		}

		public List<Tile> GetDirectlyAdjacentTiles()
		{
			var worldCoords = GetWorldPosition();
			var map = world.noiseMaps[MapCategory.TERRAIN];
			var tileList = new List<Tile>();

			if(worldCoords.x - 1 > 0)
			{
				tileList.Add(world.GetTileAtWorldPosition(new Vector2Int(worldCoords.x - 1, worldCoords.y)));
			}
			if (worldCoords.x + 1 < map.GetLength(0))
			{
				tileList.Add(world.GetTileAtWorldPosition(new Vector2Int(worldCoords.x + 1, worldCoords.y)));
			}
			if (worldCoords.y - 1 > 0)
			{
				tileList.Add(world.GetTileAtWorldPosition(new Vector2Int(worldCoords.x, worldCoords.y - 1)));
			}
			if (worldCoords.y + 1 < map.GetLength(1))
			{
				tileList.Add(world.GetTileAtWorldPosition(new Vector2Int(worldCoords.x, worldCoords.y + 1)));
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

		public List<City> GetCities()
		{
			var cities = new List<City>();
			foreach (Landmark landmark in landmarks)
			{
				if (landmark is City city)
				{
					cities.Add(city);
				}
			}

			return cities;
		}

		public void ChangeControl(Faction newFaction)
		{
			if(controller != null)
			{
				for(int i = 0; i < landmarks.Count; i++)
				{
					if(landmarks[i] is City city)
					{
						controller.RemoveCity(city);
						newFaction.AddCity(city);
					}
				}
				controller.territory.Remove(this);
			}

			newFaction.territory.Add(this);
			controller = newFaction;
			for (int i = 0; i < landmarks.Count; i++)
			{
				landmarks[i].Faction = newFaction;
			}
		}

		public static int GetDistanceBetweenTiles(Tile tileA, Tile tileB)
		{
			var posA = tileA.GetWorldPosition();
			var posB = tileB.GetWorldPosition();
			return (int)(posA - posB).magnitude;
		}

		public static City GetClosestCityToTile(List<City> cities, Tile tile)
		{
			var distance = int.MaxValue;
			City closestCity = null;

			for(int i = 0; i < cities.Count; i++)
			{
				var checkDist = GetDistanceBetweenTiles(cities[i].tile, tile);
				if (checkDist < distance)
				{
					distance = checkDist;
					closestCity = cities[i];
				}
			}

			return closestCity;
		}

		public Direction DetermineAdjacentRelativeDirection(Tile other)
		{
			var myWorldPos = GetWorldPosition();
			var otherWorldPos = other.GetWorldPosition();

			if(otherWorldPos.x > myWorldPos.x)
			{
				return Direction.EAST;
			}
			else if(otherWorldPos.x < myWorldPos.x)
			{
				return Direction.WEST;
			}
			else if(otherWorldPos.y > myWorldPos.y)
			{
				return Direction.NORTH;
			}
			else
			{
				return Direction.SOUTH;
			}
		}
	}
}
