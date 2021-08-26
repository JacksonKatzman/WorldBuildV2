using Game.Generators.Items;
using System.Collections.Generic;
using System.Linq;

namespace Game.Landmarks
{
	public class MeteorCrater : Landmark
	{
		public Material material;
		public int remainingResource;
		public List<Item> items;

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