using Game.Generators.Items;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class CreateRelicModifier : IncidentModifier
	{
		public List<MaterialUse> materialUses;
		public int numActives;
		private Item itemCreated;

		public CreateRelicModifier(List<IIncidentTag> tags, float probability, List<MaterialUse> materialUses, int numActives) : base(tags, probability)
		{
			this.materialUses = materialUses;
			this.numActives = numActives;
		}

		public override void Setup()
		{
			base.Setup();
			itemCreated = ItemGenerator.GenerateRelic(materialUses, numActives);
			ProvideModifierInfo(x => (x as IInventoryContainer)?.Inventory.Add(itemCreated));
		}

		public override void LogModifier()
		{
			incidentLogs.Add(itemCreated.Name + " was created.");
		}
	}
}