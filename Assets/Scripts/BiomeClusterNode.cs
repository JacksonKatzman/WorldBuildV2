using System.Collections.Generic;
using UnityEngine;
using Game.WorldGeneration;
using Edge = System.Tuple<int, int>;
using System;

public class BiomeClusterNode
{
   public Biome biome;
   public List<Vector2Int> tiles;

	public BiomeClusterNode(Biome biome, Vector2Int firstVector)
	{
		this.biome = biome;
		this.tiles = new List<Vector2Int>();
		tiles.Add(firstVector);
	}
}

public class EdgeComparer : IEqualityComparer<Edge>
{
	public bool Equals(Edge x, Edge y)
	{
		bool b1 = (x.Item1 == y.Item1 && x.Item2 == y.Item2);
		bool b2 = (x.Item2 == y.Item1 && x.Item1 == y.Item2);
		return b1 || b2;
	}

	public int GetHashCode(Edge obj)
	{
		return 0;
	}
}
