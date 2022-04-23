using Game.Generators.Items;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class CreateRelicModifier : IncidentModifier
	{
		public List<MaterialUse> materialUses;
		public int numActives;

		public CreateRelicModifier(List<IIncidentTag> tags, float probability, List<MaterialUse> materialUses, int numActives) : base(tags, probability)
		{
			this.materialUses = materialUses;
			this.numActives = numActives;
		}

		public override void Setup()
		{
			base.Setup();
			var item = ItemGenerator.GenerateRelic(materialUses, numActives);
			ProvideModifierInfo(x => (x as IInventoryContainer)?.Inventory.Add(item));
		}
	}
}