using ConcaveHull;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Terrain
{
    public class CellVertexInfo
	{
		public CellVertexNodeList terrainNodes, riverNodes, roadNodes, waterNodes, shoreNodes, estuaryNodes;

		public CellVertexInfo()
        {
			terrainNodes = new CellVertexNodeList();
			riverNodes = new CellVertexNodeList();
			roadNodes = new CellVertexNodeList();
			waterNodes = new CellVertexNodeList();
			shoreNodes = new CellVertexNodeList();
			estuaryNodes = new CellVertexNodeList();
        }
		public void Clear()
		{
			terrainNodes.Clear();
			riverNodes.Clear();
			roadNodes.Clear();
			waterNodes.Clear();
			shoreNodes.Clear();
			estuaryNodes.Clear();
		}
	}

	public class CellVertexNodeList
	{
		public List<Node> nodes;

		public CellVertexNodeList()
		{
			nodes = new List<Node>();
		}
		public void Clear()
		{
			nodes.Clear();
		}

		public void AddVerts(params Vector3[] args)
        {
			foreach(var arg in args)
            {
				var perturbed = HexMetrics.Perturb(arg);
				nodes.Add(new Node(perturbed.x, perturbed.z, nodes.Count - 1));
            }
        }
	}
}