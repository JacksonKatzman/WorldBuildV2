using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Generators.Noise
{
    [System.Serializable]
    public class NoiseSettings
    {
        public Vector2Int worldSize;

        [Range(0.001f, 1000f)]
        public float scale = 1.0f;

        [Range(0, 10)]
        public int octaves;

        [Range(0.0f, 1.0f)]
        public float persistance;

        [Range(1f, 10f)]
        public float lacunarity;

        public Vector2 offset;
    }
}