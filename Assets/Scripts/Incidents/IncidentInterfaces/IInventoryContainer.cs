using Game.Generators.Items;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface IInventoryContainer
	{
		public List<Item> Inventory
		{
			get;
		}
	}
}