using Game.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Terrain
{
    public class FoliageManager : MonoBehaviour
    {
        public static FoliageManager Instance { get; private set; }
        private Dictionary<Mesh, MeshMaterialMatrixInfo> allBatches;
        private Dictionary<Mesh, MeshMaterialMatrixInfo> visibleBatches;
        private Dictionary<HexCell, Dictionary<Mesh, MeshMaterialMatrixInfo>> cellBatches;

        private struct MeshMaterialMatrixInfo
        {
            public Material[] materials;
            public List<List<Matrix4x4>> matrices;
        }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                allBatches = new Dictionary<Mesh, MeshMaterialMatrixInfo>();
                visibleBatches = new Dictionary<Mesh, MeshMaterialMatrixInfo>();
                cellBatches = new Dictionary<HexCell, Dictionary<Mesh, MeshMaterialMatrixInfo>>();
                Instance = this;
            }
        }

        private void Update()
        {
            RenderBatches();
        }

        private void RenderBatches()
        {
            var chosenBatches = HexMetrics.mapFullyVisible ? allBatches : visibleBatches;
            if (chosenBatches.Count > 0)
            {
                foreach (var pair in chosenBatches)
                {
                    for (int i = 0; i < pair.Value.matrices.Count; i++)
                    {
                        var batch = pair.Value.matrices[i];
                        for (int j = 0; j < pair.Key.subMeshCount; j++)
                        {
                            Graphics.DrawMeshInstanced(pair.Key, j, pair.Value.materials[j], batch);
                        }
                    }
                }
            }
        }

        public void UpdateFoliageVisibility(HexCell cell)
        {
            if (cellBatches.ContainsKey(cell))
            {
                var cellInfo = cellBatches[cell];
                foreach (var pair in cellInfo)
                {
                    foreach (var matrix in pair.Value.matrices[0])
                    {
                        AddToBatches(pair.Key, pair.Value.materials, matrix, ref visibleBatches);
                    }
                }
            }
        }

        public void AddToBatches(Mesh mesh, Material[] materials, AssetPositionInformation info, HexCell cell, bool jitter = false)
        {
            Vector3 position = info.position;
            if(jitter)
            {
                var variance = HexMetrics.outerRadius / 20;
                position = new Vector3(info.position.x + SimRandom.RandomFloat(-variance, variance), info.position.y, info.position.z + SimRandom.RandomFloat(-variance, variance));
            }

            var matrix = Matrix4x4.TRS(info.position, Quaternion.Euler(0.0f, 360.0f * SimRandom.RandomFloat01(), 0.0f), info.scale);

            if(!cellBatches.ContainsKey(cell))
            {
                cellBatches.Add(cell, new Dictionary<Mesh, MeshMaterialMatrixInfo>());
            }

            if(!cellBatches[cell].ContainsKey(mesh))
            {
                cellBatches[cell].Add(mesh, new MeshMaterialMatrixInfo() { materials = materials, matrices = new List<List<Matrix4x4>>() });
                cellBatches[cell][mesh].matrices.Add(new List<Matrix4x4>());
            }

            cellBatches[cell][mesh].matrices[0].Add(matrix);

            AddToBatches(mesh, materials, matrix, ref allBatches);
        }

        private void AddToBatches(Mesh mesh, Material[] materials, Matrix4x4 matrix, ref Dictionary<Mesh, MeshMaterialMatrixInfo> batchCollection)
        {
            if (!batchCollection.ContainsKey(mesh))
            {
                batchCollection.Add(mesh, new MeshMaterialMatrixInfo() { materials = materials, matrices = new List<List<Matrix4x4>>() });
                batchCollection[mesh].matrices.Add(new List<Matrix4x4>());
            }

            var batch = batchCollection[mesh];
            if (batch.matrices[batch.matrices.Count - 1].Count >= 1000)
            {
                batch.matrices.Add(new List<Matrix4x4>());
            }

            batch.matrices[batch.matrices.Count - 1].Add(matrix);
        }
    }
}