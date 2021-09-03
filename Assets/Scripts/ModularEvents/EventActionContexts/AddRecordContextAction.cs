using Game.Data.EventHandling.EventRecording;
using System.Collections;
using UnityEngine;

namespace Game.ModularEvents
{
	public class AddRecordContextAction : IEventActionContext
	{
		public string recordContext;
		public void Activate(Person person, EventRecord record)
		{
			record.AddContext(recordContext, person);
		}
	}
}