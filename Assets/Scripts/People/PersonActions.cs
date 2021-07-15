using System.Collections;
using UnityEngine;
using Game.Data.EventHandling;

namespace Game.People
{
	public abstract class PersonActions
	{
		public static void TestPersonAction(Person person)
		{
			EventManager.Instance.Dispatch(new BaseRPEvent(person, "Base Event"));
		}
	}
}