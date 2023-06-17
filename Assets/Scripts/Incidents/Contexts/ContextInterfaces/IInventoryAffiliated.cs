using Game.Generators.Items;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface IInventoryAffiliated
	{
		Inventory CurrentInventory { get; }
	}
}