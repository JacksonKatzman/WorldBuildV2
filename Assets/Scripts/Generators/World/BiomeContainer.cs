using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.WorldGeneration;

[CreateAssetMenu(fileName = "BiomeContainer", menuName = "ScriptableObjects/BiomeContainer", order = 1)]
public class BiomeContainer : ScriptableObject
{
	public List<Biome> biomes;
}