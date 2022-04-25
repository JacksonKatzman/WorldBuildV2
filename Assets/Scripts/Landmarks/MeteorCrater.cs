using Game.Generators.Items;
using Game.Incidents;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Landmarks
{
	public class MeteorCrater : Landmark, IInventoryContainer
	{
		public Material material;
		public int remainingResource;
		private List<Item> items;

		public List<Item> Inventory => items;

		public MeteorCrater()
		{
		}

		public MeteorCrater(Material material, params Item[] items)
		{
			this.material = material;
			this.items = items.ToList();

			remainingResource = SimRandom.RandomRange(50, 500);
		}

		public override void AdvanceTime()
		{
			if(faction != null && remainingResource > 0)
			{
				remainingResource--;
				//add some bonus based on resource mined

				//also add a chance to find any remaining item while they are mining
			}
		}
	}
}