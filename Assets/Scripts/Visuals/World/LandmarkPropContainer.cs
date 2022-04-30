using System.Collections.Generic;
using UnityEngine;
using Sirenix.Serialization;
using Sirenix.OdinInspector;
using System;

namespace Game.Visuals
{
	[CreateAssetMenu(fileName = nameof(LandmarkPropContainer), menuName = "ScriptableObjects/Containers/" + nameof(LandmarkPropContainer), order = 1)]
	public class LandmarkPropContainer : SerializedScriptableObject
	{
		[SerializeField]
		List<LandmarkPropContainerNode> nodes;

		public Dictionary<Type, List<GameObject>> props => BuildDictionary();

		private Dictionary<Type, List<GameObject>> BuildDictionary()
		{
			var dictionary = new Dictionary<Type, List<GameObject>>();

			foreach(var node in nodes)
			{
				dictionary.Add(node.type, node.props);
			}

			return dictionary;
		}
	}
}