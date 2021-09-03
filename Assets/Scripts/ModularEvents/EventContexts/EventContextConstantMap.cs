using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.ModularEvents
{
	public enum EventContextType { HOSTILE_CREATURE, TEST }
	public static class EventContextConstantMap
	{
		public static readonly Dictionary<EventContextType, Type> Map = new Dictionary<EventContextType, Type>
		{
			{EventContextType.HOSTILE_CREATURE, typeof(HostileCreatureContext) },
			{EventContextType.TEST, typeof(TestContext) }
		};
	}
}