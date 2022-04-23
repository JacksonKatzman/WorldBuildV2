using Game.Generators.Items;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Creatures
{
	public interface ICreature
	{
		public List<Item> Inventory
		{
			get;
		}
	}
}