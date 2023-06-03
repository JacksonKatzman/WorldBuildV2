using Game.Enums;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Terrain
{
	[CreateAssetMenu(fileName = "AssetCollection", menuName = "ScriptableObjects/Simulation/Asset Collection", order = 1)]
	public class AssetCollection : SerializedScriptableObject
	{
		public HexFeatureCollection[] urbanCollections;
		public HexFeatureCollection[] ruralCollections;
		public HexFeatureCollection[] plantCollections;

		public Dictionary<LandmarkType, List<Transform>> landmarkCollections;

		public Transform[] bridges;

		public Transform wallTower;
	}
}