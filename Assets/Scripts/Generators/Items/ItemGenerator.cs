using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.Generators.Items
{
	public static class ItemGenerator
	{
		public static Relic GenerateRelic(List<MaterialUse> uses, int actives, int passives)
		{
			var name = NameGenerator.GenerateRelicName();

			var randomIndex = SimRandom.RandomRange(0, uses.Count);
			var material = DataManager.Instance.MaterialGenerator.GetRandomMaterialByUse(uses[randomIndex], true);

			var activesList = new List<MethodInfo>();
			for(int i = 0; i < actives; i++)
			{
				activesList.Add(SimAIManager.Instance.GetRandomLoreEvent());
			}

			var passivesList = new List<MethodInfo>();
			for (int i = 0; i < actives; i++)
			{

			}

			var relic = new Relic(name, material, activesList, passivesList);
			return relic;
		}
	}
}