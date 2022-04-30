using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.Generators.Items
{
	public static class ItemGenerator
	{
		public static Item GenerateRelic(List<MaterialUse> uses, int actives)
		{
			var name = NameGenerator.GenerateRelicName();

			var randomIndex = SimRandom.RandomRange(0, uses.Count);
			var material = DataManager.Instance.MaterialGenerator.GetRandomMaterialByUse(uses[randomIndex], true);

			var activesList = new List<MethodInfo>();
			for(int i = 0; i < actives; i++)
			{
				activesList.Add(SimAIManager.Instance.GetRandomLoreEvent());
			}

			var relic = new Item(name, material, activesList, new List<string>());
			return relic;
		}
	}
}