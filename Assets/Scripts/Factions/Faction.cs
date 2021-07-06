using Game.Enums;
using Game.WorldGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Factions
{
	public class Faction : ITimeSensitive
	{
		private static int STARTING_INFLUENCE = 10;

		public string name;
		public City capitalCity;
		public Government government;
		public List<Tile> territory;
		public int influence;
		public World world;

		public Faction(Tile startingTile, float food, int population)
		{
			name = NameGenerator.GeneratePersonFirstName(WorldHandler.Instance.PrimaryNameContainer, Gender.NEITHER);

			territory = new List<Tile>();
			world = startingTile.world;
			territory.Add(startingTile);
			capitalCity = startingTile.SpawnCity(this, food, population);
			influence = STARTING_INFLUENCE;

			OutputLogger.LogFormatAndPause("{0} faction has been created in {1} City.", LogSource.FACTION, name, capitalCity.name);
		}
		public void AdvanceTime()
		{
			foreach(Tile tile in territory)
			{
				foreach(Landmark landmark in tile.landmarks)
				{
					landmark.AdvanceTime();
				}
			}
		}
		public bool SpawnCityWithinRadius(Tile tile, float foodAmount, int population)
		{
			int radius = 4 + influence / 100;
			var possibleTiles = tile.GetAllTilesInRadius(radius);
			bool spawned = false;
			int attempts = 0;
			while (!spawned && attempts < 10)
			{
				var randomIndex = WorldHandler.Instance.RandomRange(0, possibleTiles.Count);
				var chosenTile = possibleTiles[randomIndex];
				var tileController = world.GetFactionThatControlsTile(chosenTile);
				if (chosenTile.baseFertility > 0.5f && chosenTile.landmarks.Count == 0 && (tileController == this || tileController == null))
				{
					if(tileController == this)
					{
						chosenTile.SpawnCity(this, foodAmount, population);
					}
					else
					{
						world.CreateNewFaction(chosenTile, foodAmount, population);
					}
					spawned = true;
					OutputLogger.LogFormatAndPause("Spawned city in chunk ({0},{1}) in tile ({2},{3})).",
						LogSource.WORLDGEN, chosenTile.chunk.coords.x, chosenTile.chunk.coords.y, chosenTile.coords.x, chosenTile.coords.y);
					OutputLogger.LogFormat("World Coords are: {0},{1}.", LogSource.WORLDGEN, chosenTile.GetWorldPosition(), tile.GetWorldPosition());
				}
				attempts++;
			}
			return spawned;
		}

		//TODO: Create basic government types and spawn governments with those types
		//TODO: Define a way that factions/governments generate influence
		//TODO: Use influence and faction pressure to determine tile aquisition 
	}
}