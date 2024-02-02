using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Terrain
{
    public class HexTerrainGuide : MonoBehaviour
    {
        public HexCell cell;
        public HexGridChunk chunk;
        public Texture2D noiseSource;
        public Transform cellPrefab;
        public Transform guidelineParent;

        [Button("Init Meshes")]
        public void InitMeshes()
        {
            HexMetrics.noiseSource = noiseSource;
            chunk.TestInitMeshes();
        }

        [Button("Triangulate")]
        public void Triangulate()
        {
            chunk.DebugTriangulate();
        }

        [Button("Remove Rivers")]
        public void RemoveRivers()
        {
            cell.RemoveRiver();
        }

        [Button("Straight River")]
        public void StraightRiver()
        {
            RemoveRivers();
            cell.SetOutgoingRiver(HexDirection.SW);
            cell.GetNeighbor(HexDirection.NE).SetOutgoingRiver(HexDirection.SW);
        }

        [Button("Curved River")]
        public void CurvedRiver()
        {
            RemoveRivers();
            cell.SetOutgoingRiver(HexDirection.SE);
            cell.GetNeighbor(HexDirection.NE).SetOutgoingRiver(HexDirection.SW);
        }

        [Button("Sharp River")]
        public void SharpRiver()
        {
            RemoveRivers();
            cell.SetOutgoingRiver(HexDirection.E);
            cell.GetNeighbor(HexDirection.NE).SetOutgoingRiver(HexDirection.SW);
        }

        public bool[] roads = new bool[6];

        [Button("Update Roads")]
        public void UpdateRoads()
        {
            cell.RemoveRoads();
            for (int i = 0; i < roads.Length; i++)
            {
                if (roads[i])
                {
                    cell.AddRoad((HexDirection)i);
                }
            }

            Triangulate();
        }

        [Button("Create Cells")]
        public void CreateCells()
        {
            if (cell == null)
            {
                chunk.cells = new HexCell[7];
                chunk.cells[0] = cell = CreateCell(0, 0);
                cell.SetNeighbor(HexDirection.NE, chunk.cells[1] = CreateCell(0, 1));
                cell.SetNeighbor(HexDirection.E, chunk.cells[2] = CreateCell(1, 0));
                cell.SetNeighbor(HexDirection.SE, chunk.cells[3] = CreateCell(0, -1));
                cell.SetNeighbor(HexDirection.SW, chunk.cells[4] = CreateCell(-1, -1));
                cell.SetNeighbor(HexDirection.W, chunk.cells[5] = CreateCell(-1, 0));
                cell.SetNeighbor(HexDirection.NW, chunk.cells[6] = CreateCell(-1, 1));

                foreach(var c in chunk.cells)
                {
                    c.chunk = chunk;
                }
            }
        }


		HexCell CreateCell(int x, int z)
		{
			Vector3 position;
			position.x = (x + z * 0.5f - z / 2) * (HexMetrics.innerRadius * 2f);
			position.y = 0f;
			position.z = z * (HexMetrics.outerRadius * 1.5f);

			HexCell cell = Instantiate(cellPrefab).GetComponent<HexCell>();
			cell.transform.localPosition = position;
			cell.coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
            cell.transform.parent = guidelineParent;
            return cell;
		}
	}
}