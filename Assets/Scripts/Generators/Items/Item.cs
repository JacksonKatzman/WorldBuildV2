using Game.Enums;
using Game.Incidents;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Generators.Items
{
	abstract public class Item : InertIncidentContext
	{
		public override Type ContextType => typeof(Item);
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