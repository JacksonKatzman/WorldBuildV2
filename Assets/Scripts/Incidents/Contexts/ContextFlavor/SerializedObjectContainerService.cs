using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	public class SerializedObjectContainerService
	{
		public SerializedObjectContainer container;

		private static SerializedObjectContainerService instance;
		public static SerializedObjectContainerService Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new SerializedObjectContainerService();
				}
				return instance;
			}
		}

		private SerializedObjectContainerService()
		{
			container = Resources.Load<SerializedObjectContainer>("ScriptableObjects/Data/ObjectData");
		}
	}

	[CreateAssetMenu(fileName = nameof(SerializedObjectContainer), menuName = "ScriptableObjects/Data/" + nameof(SerializedObjectContainer), order = 1)]
	public class SerializedObjectContainer : SerializedScriptableObject
	{
		public Dictionary<Type, Dictionary<string, SerializedScriptableObject>> objects;
	}
}