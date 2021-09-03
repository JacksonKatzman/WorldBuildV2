using System.Collections;
using UnityEngine;

namespace Game.ModularEvents
{
	[System.Serializable]
	public class EventActionContextContainer
	{
		[SerializeReference]
		public IEventActionContext eventAction;
	}
}