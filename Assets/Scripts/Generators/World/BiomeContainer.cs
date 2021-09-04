using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.WorldGeneration;

[CreateAssetMenu(fileName = nameof(BiomeContainer), menuName = "ScriptableObjects/Biomes/" + nameof(BiomeContainer), order = 2)]
public class BiomeContainer : ScriptableObject
{
	public List<Biome> biomes;
}