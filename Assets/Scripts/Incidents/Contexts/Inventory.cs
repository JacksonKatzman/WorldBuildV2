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
	
	//the issue is caused by the fact that when we go searchinf for an Inventory, it adds that type to the context dict
	//and then the iteration cant continue when it looks for the next type in that collection
	//but i doubt i want to add every inventory to that list... so might need a better way to access inventories

	//its actually likely the Item type being added not the inventory
}