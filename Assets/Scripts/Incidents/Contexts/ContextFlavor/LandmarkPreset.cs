using Game.Enums;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{

    [CreateAssetMenu(fileName = nameof(LandmarkPreset), menuName = "ScriptableObjects/Data/" + nameof(LandmarkPreset), order = 1)]
	public class LandmarkPreset : SerializedScriptableObject
	{
		public List<LandmarkTrait> landmarkTraits;
		public List<Transform> models;

		public LandmarkPreset()
		{
			landmarkTraits = new List<LandmarkTrait>();
			models = new List<Transform>();
		}
	}
}