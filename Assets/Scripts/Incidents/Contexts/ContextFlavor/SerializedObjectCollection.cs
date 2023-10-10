using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	[CreateAssetMenu(fileName = nameof(SerializedObjectCollection), menuName = "ScriptableObjects/Data/" + nameof(SerializedObjectCollection), order = 1)]
	public class SerializedObjectCollection : SerializedScriptableObject
	{
		public Dictionary<string, SerializedScriptableObject> objects;
	}
}