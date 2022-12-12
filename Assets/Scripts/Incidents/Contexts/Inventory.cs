using Game.Generators.Items;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class Inventory : InertIncidentContext
	{
		public override Type ContextType => typeof(Inventory);
		public List<Item> Items { get; set; }

		public Inventory() 
		{
			Items = new List<Item>();
		}
		public Inventory(List<Item> items)
		{
			Items = items;
		}
	}
}