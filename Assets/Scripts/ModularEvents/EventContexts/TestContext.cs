using Game.Data.EventHandling.EventRecording;
using Game.Enums;
using System.Collections;
using UnityEngine;

namespace Game.ModularEvents
{
	public class TestContext : IEventContext
	{
		public string test;
		public int GetRollMargin(Person person, StatType statType)
		{
			return 0;
		}

		public void RunEvent(Person person, EventRecord record)
		{
			throw new System.NotImplementedException();
		}
	}
}