using Game.Enums;
using Game.Incidents;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Generators.Items
{
	abstract public class Item : InertIncidentContext, IInventoryAffiliated
	{
		public override Type ContextType => typeof(Item);

		public Inventory CurrentInventory { get; set; }
		public ItemValue Value { get; set; }
		public int Points { get; set; }

		abstract public void RollStats(int points);

		public static Item Create(ItemType itemType)
		{
			var type = itemType.GetType();
			if(type == typeof(WeaponType))
			{
				return new Weapon(itemType as WeaponType);
			}
			else if(type == typeof(ArmorType))
			{
				return new Armor(itemType as ArmorType);
			}
			else
			{
				return new Trinket();
			}
		}
	}

	[System.Serializable]
	public class ItemValue
	{
		public int amount;
		public CoinType coinType;

		public ItemValue(int amount, CoinType coinType)
		{
			this.amount = amount;
			this.coinType = coinType;
		}
	}
}