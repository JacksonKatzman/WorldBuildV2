using System.Collections;
using UnityEngine;
using Game.Enums;
using System;
using Game.Data.EventHandling.EventRecording;

namespace Game.ModularEvents
{
	public interface IEventContext
	{
		void RunEvent(Person person, EventRecord record);
		int GetRollMargin(Person person, StatType statType);
	}
}