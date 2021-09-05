using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Visuals
{
	[System.Serializable]
	public class LandmarkPropContainerNode
	{
		[SerializeReference]
		Landmark landmark;

		public List<GameObject> props;

		public Type type => landmark.GetType();
	}
}