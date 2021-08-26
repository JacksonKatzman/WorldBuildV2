using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public abstract class LoreEvents
	{
		public static void TestLoreEvent()
		{
			OutputLogger.LogFormat("Test Lore Event.", Game.Enums.LogSource.EVENT);
		}
	}

	public abstract class LoreEventHelpers
	{

	}
}