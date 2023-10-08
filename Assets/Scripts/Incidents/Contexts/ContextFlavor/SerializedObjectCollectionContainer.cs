using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	[CreateAssetMenu(fileName = nameof(SerializedObjectCollectionContainer), menuName = "ScriptableObjects/Data/" + nameof(SerializedObjectCollectionContainer), order = 1)]
	public class SerializedObjectCollectionContainer : SerializedScriptableObject
	{
		public Dictionary<Type, SerializedObjectCollection> collections;
	}
}