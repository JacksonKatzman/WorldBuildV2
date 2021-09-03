using Game.Data.EventHandling.EventRecording;
using System.Collections;
using UnityEngine;

namespace Game.ModularEvents
{
	public interface IEventActionContext
	{
		void Activate(Person person, EventRecord record);
	}
}