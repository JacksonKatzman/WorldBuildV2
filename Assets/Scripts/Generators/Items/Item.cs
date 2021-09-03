using Game.Data.EventHandling.EventRecording;
using Game.Enums;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.Generators.Items
{
	public class Item : IRecordable
	{
		public virtual string Name => name;

		protected string name;
		protected Material material;

		public List<MethodInfo> simulationEffects;
		public List<string> gameEffects;

		public Item(string name, Material material, List<MethodInfo> simulationEffects, List<string> gameEffects)
		{
			this.name = name;
			this.material = material;
			this.simulationEffects = simulationEffects;
			this.gameEffects = gameEffects;
		}

		public void Activate()
		{
			foreach (var method in simulationEffects)
			{
				method.Invoke(null, new object[] { });
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