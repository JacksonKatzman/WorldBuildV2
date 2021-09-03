using Game.Enums;
using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Game.Data.EventHandling.EventRecording;

namespace Game.ModularEvents
{
	[System.Serializable]
	public class ModularEventBranch
	{
		public int rollThreshold;
		//list of actions

		public List<EventActionContextContainer> eventActions;

		//whether or not it spawns a new event
		public bool hasFollowUpEvent;

		[ShowIf("hasFollowUpEvent", true), Range(0.0f, 1.0f)]
		public float eventChance;

		[ShowIf("hasFollowUpEvent", true)]
		public EventContextContainer outputEventContext;
		//context for that event

		public void RunBranch(Person person, EventRecord record)
		{
			foreach(var action in eventActions)
			{
				action.eventAction.Activate(person, record);
			}
		}
	}
}