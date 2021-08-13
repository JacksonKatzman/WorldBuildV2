using Game.Data.EventHandling;
using Game.Enums;
using Game.Factions;
using Game.WorldGeneration;
using System.Collections;
using UnityEngine;

namespace Game.Generators
{
	public static class FactionGenerator
	{
		private static float STARTING_FOOD = 100.0f;
		private static int STARTING_POPULATION = 100;
		private static int MAX_SPAWN_ATTEMPTS = 100;
		public static void SpawnFaction(World world)
		{
			bool spawned = false;
			int attempts = 0;
			while (!spawned && attempts < MAX_SPAWN_ATTEMPTS)
			{
				var randomXIndex = SimRandom.RandomRange(0, world.noiseMaps[Enums.MapCategory.TERRAIN].GetLength(0));
				var randomYIndex = SimRandom.RandomRange(0, world.noiseMaps[Enums.MapCategory.TERRAIN].GetLength(1));
				var chosenTile = world.GetTileAtWorldPosition(new Vector2Int(randomXIndex, randomYIndex));

				if (LandmarkGenerator.IsSuitableCityLocation(chosenTile, 0.5f, 0.2f))
				{
					SpawnFaction(chosenTile, STARTING_FOOD, STARTING_POPULATION);
					spawned = true;
				}
				attempts++;
			}

			if (attempts >= MAX_SPAWN_ATTEMPTS)
			{
				//Debug.LogFormat("Failed to spawn city in chunk ({0},{1})", coords.x, coords.y);
			}
		}

		public static void SpawnFaction(Tile tile, float foodAmount, int population)
		{
			var faction = new FactionSimulator(tile, foodAmount, population);
			EventManager.Instance.Dispatch(new FactionCreatedEvent(faction));

			OutputLogger.LogFormat("Spawned faction in chunk ({0},{1}) in tile ({2},{3})).",
						LogSource.WORLDGEN, tile.chunk.coords.x, tile.chunk.coords.y, tile.coords.x, tile.coords.y);
		}

		public static void DestroyFaction(FactionSimulator faction)
		{
			for(int i = 0; i < faction.territory.Count; i++)
			{
				faction.territory[i].controller = null;
			}
			faction.territory.Clear();
			EventManager.Instance.Dispatch(new FactionDestroyedEvent(faction));
		}
	}
}