using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	[CreateAssetMenu(fileName = nameof(LandmarkPreset), menuName = "ScriptableObjects/Visuals/" + nameof(LandmarkPreset), order = 1)]
	public class LandmarkPreset : SerializedScriptableObject
	{
		public List<GameObject> landmarkPrefabs;
	}
}