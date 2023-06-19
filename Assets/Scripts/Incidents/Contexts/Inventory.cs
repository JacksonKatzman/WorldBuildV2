using Game.Generators.Items;
using Game.Utilities;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class Inventory : InertIncidentContext, IInventoryAffiliated
	{
		public override Type ContextType => typeof(Inventory);
		public List<Item> Items { get; set; }
		public Inventory CurrentInventory => this;

		public Inventory() 
		{
			Items = new List<Item>();
		}
		public Inventory(List<Item> items)
		{
			Items = items;
		}

		public override void LoadContextProperties()
		{
			SaveUtilities.ConvertIDsToContexts<Item>(contextIDLoadBuffers["Items"]);
		}
	}
}