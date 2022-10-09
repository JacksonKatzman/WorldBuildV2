using Game.Creatures;
using Game.Generators.Items;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class CreaturesGetItemsModifier : IncidentModifier, ICreatureContainer, IInventoryContainer
	{
		private List<ICreature> creatures;
		private List<Item> items;
		public List<ICreature> Creatures => creatures;

		public List<Item> Inventory => items;

		public CreaturesGetItemsModifier(List<IIncidentTag> tags, float probability) : base(tags, probability)
		{
			creatures = new List<ICreature>();
			items = new List<Item>();
		}

		public override void Run(OldIncidentContext context)
		{
			base.Run(context);
			creatures.ForEach(x => x.Inventory.AddRange(items));
		}
	}
}