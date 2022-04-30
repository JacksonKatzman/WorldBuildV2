using Game.WorldGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Visuals
{
    public class AStarTile
    {
		public Tile tile;
        public Vector2Int pos;
        public float Cost;
        public int Distance;
        public float CostDistance => Cost + Distance;
        public AStarTile parent;

		public AStarTile(Tile tile, Vector2Int pos, float cost, int distance, AStarTile parent)
		{
			this.tile = tile;
			this.pos = pos;
			Cost = cost;
			Distance = distance;
			this.parent = parent;
		}

		public void SetDistance(Vector2Int target)
		{
            this.Distance = (int)Vector2Int.Distance(pos, target);
		}
    }
}
