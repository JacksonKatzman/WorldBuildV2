using Game.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Terrain
{
    [CreateAssetMenu(fileName = nameof(BiomeData), menuName = "ScriptableObjects/Biomes/" + nameof(BiomeData), order = 1)]
    public class BiomeData : SerializedScriptableObject
    {
        public BiomeTerrainType terrainType;
        public List<WeightedTexture> textures;
        public Material terrainMaterial;
        public List<GameObject> mountainAssets;
        public List<GameObject> riverStartMountainAssets;
        public List<GameObject> hillAssets;
        public List<GameObject> riverStartHillAssets;
        public List<GameObject> foliageAssets;

        public int minHeight, maxHeight;
        [Range(0.0f, 1.0f)]
        public float minTemperature, maxTemperature;
        [Range(0.0f, 1.0f)]
        public float minMoisture, maxMoisture;
        public int mountainThreshold = 6;
        public int hillThreshold = 5;
        [Range(0.0f, 1.0f)]
        public float plantLevel;
        [Range(0.0f, 0.2f)]
        public float plantLevelVarianceMax;
        [Range(-0.2f, 0.0f)]
        public float plantLevelVarianceMin;
        [Range(0, 200)]
        public int grassDensity;

        public int GetTextureIndex()
        {
            var total = textures.Sum(x => x.weight);
            var random = SimRandom.RandomRange(1, total + 1);
            for(int i = 0; i < textures.Count; i++)
            {
                if((random -= textures[i].weight) <= 0)
                {
                    return i;
                }
            }
            return textures.Count - 1;

        }
    }

    [Serializable]
    public class WeightedTexture
    {
        public Texture2D texture;
        public int weight = 1;
    }
}