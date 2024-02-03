using Game.Utilities;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Terrain
{
    public class FoliageManager : MonoBehaviour
    {
        public static FoliageManager Instance { get; private set; }
        private Dictionary<Mesh, MeshMaterialMatrixInfo> batches;

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
                batches = new Dictionary<Mesh, MeshMaterialMatrixInfo>();
                Instance = this;
            }
        }

        private void Update()
        {
            RenderBatches();
        }

        private void RenderBatches()
        {
            if (batches.Count > 0)
            {
                foreach (var pair in batches)
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

        public void AddToBatches(Mesh mesh, Material[] materials, AssetPositionInformation info)
        {
            if (!batches.ContainsKey(mesh))
            {
                batches.Add(mesh, new MeshMaterialMatrixInfo() { materials = materials, matrices = new List<List<Matrix4x4>>() });
                batches[mesh].matrices.Add(new List<Matrix4x4>());
            }

            var batch = batches[mesh];
            if (batch.matrices[batch.matrices.Count - 1].Count >= 1000)
            {
                batch.matrices.Add(new List<Matrix4x4>());
            }

            batch.matrices[batch.matrices.Count - 1].Add(Matrix4x4.TRS(info.position, Quaternion.Euler(0.0f, 360.0f * SimRandom.RandomFloat01(), 0.0f), info.scale));
        }
    }
}