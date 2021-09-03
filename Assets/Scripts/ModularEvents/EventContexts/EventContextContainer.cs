using Game.Enums;
using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using Sirenix.OdinInspector;

namespace Game.ModularEvents
{
	[System.Serializable]
	public class EventContextContainer
	{
		public Type contextType => contextObject.GetType();

		[SerializeReference]
		public IEventContext contextObject;

		[ShowIf("contextObject")]
		public PriorityType priorityType;
	}
}