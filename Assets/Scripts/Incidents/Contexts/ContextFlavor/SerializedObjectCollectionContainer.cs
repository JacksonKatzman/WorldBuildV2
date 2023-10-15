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

		[Button("Compile Object Collection Dictionary")]
		private void CompileObjectCollectionDictionary()
		{
			var resources = Resources.LoadAll("ScriptableObjects/Data", typeof(SerializedObjectCollection));
			foreach (var resource in resources)
			{
				var typedResource = resource as SerializedObjectCollection;
				if (collections == null)
				{
					collections = new Dictionary<Type, SerializedObjectCollection>();
				}

				if (!collections.ContainsKey(typedResource.objectType))
				{
					collections.Add(typedResource.objectType, typedResource);
				}
			}
		}
	}
}