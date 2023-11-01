using Game.Enums;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	[CreateAssetMenu(fileName = nameof(LandmarkPreset), menuName = "ScriptableObjects/Data/" + nameof(LandmarkPreset), order = 1)]
	public class LandmarkPreset : SerializedScriptableObject
	{
		public List<LandmarkTag> landmarkTags;
		public List<Transform> models;

		public LandmarkPreset()
		{
			landmarkTags = new List<LandmarkTag>();
			models = new List<Transform>();
		}
	}
}