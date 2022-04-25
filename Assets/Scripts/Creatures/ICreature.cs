using Game.Generators.Items;
using System.Collections;
using System.Collections.Generic;
using Game.Data.EventHandling.EventRecording;
using UnityEngine;

namespace Game.Creatures
{
	public interface ICreature : IRecordable
	{
		public List<Item> Inventory
		{
			get;
		}
	}
}