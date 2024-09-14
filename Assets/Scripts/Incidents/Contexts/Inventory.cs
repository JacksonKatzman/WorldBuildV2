using Game.Generators.Items;
using Game.Utilities;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class FactionItemList : List<Item>
	{
		private Faction faction;
		public FactionItemList(List<Item> items, Faction faction) : base(items)
		{
			this.faction = faction;
		}

		public new FactionItemList Add(Item item)
		{
			base.Add(item);
			SimRandom.RandomEntryFromList(faction.Cities).CurrentInventory.Items.Add(item);
			return this;
		}

		public new FactionItemList AddRange(IEnumerable<Item> items)
		{
			foreach (var item in items)
			{
				Add(item);
			}
			return this;
		}

		public new FactionItemList Remove(Item item)
		{
			base.Remove(item);
			foreach (var city in faction.Cities)
			{
				if (city.CurrentInventory.Items.Contains(item))
				{
					city.CurrentInventory.Items.Remove(item);
				}
			}
			return this;
		}
	}
	public class FactionInventory : Inventory
	{
		private Faction faction;
		private FactionItemList itemList;

		override public List<Item> Items
		{
			get
			{
				var items = new List<Item>();
				foreach (var city in faction.Cities)
				{
					items.AddRange(city.CurrentInventory.Items);
				}
				itemList = new FactionItemList(items, faction);
				return itemList;
			}
		}

		public FactionInventory(Faction faction)
		{
			this.faction = faction;
		}
	}
	public class Inventory : InertIncidentContext, IInventoryAffiliated
	{
		public override Type ContextType => typeof(Inventory);
		virtual public List<Item> Items { get; private set; }
		virtual public Inventory CurrentInventory => this;

        public override string Description => $"INVENTORY DESCRIPTION";


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