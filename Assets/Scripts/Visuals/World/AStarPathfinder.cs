using Game.Visuals;
using Game.WorldGeneration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Pathfinding
{
	public static class AStarPathfinder
	{
		private static List<AStarTile> GetWalkableTiles(Tile[,] map, AStarTile currentTile, AStarTile targetTile, Func<Tile, Tile, float> costFunction)
		{
			var possibleTiles = new List<AStarTile>();
			var adjacentTiles = currentTile.tile.GetDirectlyAdjacentTiles();
			foreach (var t in adjacentTiles)
			{
				var worldPos = t.GetWorldPosition();
				var tV = map[worldPos.x, worldPos.y];
				var newTile = new AStarTile(tV, worldPos, currentTile.Cost + costFunction(currentTile.tile, tV), 0, currentTile);
				newTile.SetDistance(targetTile.pos);
				possibleTiles.Add(newTile);
			}

			return possibleTiles;
		}

		public static List<Tile> AStarBestPath(Tile a, Tile b, Tile[,] terrainTiles, Func<Tile, Tile, float> costFunction)
		{
			var start = new AStarTile(a, a.GetWorldPosition(), 1, 0, null);
			var finish = new AStarTile(b, b.GetWorldPosition(), 1, 0, null);

			start.SetDistance(finish.pos);

			var activeTiles = new List<AStarTile>();
			activeTiles.Add(start);

			var visitedTiles = new List<AStarTile>();

			var pathway = new List<Tile>();

			while (activeTiles.Any())
			{
				var checkTile = activeTiles.OrderBy(x => x.CostDistance).First();

				if (checkTile.pos.x == finish.pos.x && checkTile.pos.y == finish.pos.y)
				{
					while (checkTile != null)
					{
						pathway.Add(checkTile.tile);
						checkTile = checkTile.parent;
					}
					break;
				}

				visitedTiles.Add(checkTile);
				activeTiles.Remove(checkTile);

				var walkableTiles = GetWalkableTiles(terrainTiles, checkTile, finish, costFunction);

				foreach (var walkableTile in walkableTiles)
				{
					//We have already visited this tile so we don't need to do so again!
					if (visitedTiles.Any(x => x.pos.x == walkableTile.pos.x && x.pos.y == walkableTile.pos.y))
						continue;

					//It's already in the active list, but that's OK, maybe this new tile has a better value (e.g. We might zigzag earlier but this is now straighter). 
					if (activeTiles.Any(x => x.pos.x == walkableTile.pos.x && x.pos.y == walkableTile.pos.y))
					{
						var existingTile = activeTiles.First(x => x.pos.x == walkableTile.pos.x && x.pos.y == walkableTile.pos.y);
						if (existingTile.CostDistance > checkTile.CostDistance)
						{
							activeTiles.Remove(existingTile);
							activeTiles.Add(walkableTile);
						}
					}
					else
					{
						//We've never seen this tile before so add it to the list. 
						activeTiles.Add(walkableTile);
					}
				}
			}

			return pathway;
		}
	}
}