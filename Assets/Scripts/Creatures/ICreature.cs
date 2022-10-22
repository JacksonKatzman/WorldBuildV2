using Game.Generators.Items;
using System.Collections.Generic;

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