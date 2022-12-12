using Game.Generators.Items;

namespace Game.Incidents
{
	public class TradeItemsAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Item> itemField;
		public InterfacedIncidentActionFieldContainer<IInventoryAffiliated> givingInventory;
		public InterfacedIncidentActionFieldContainer<IInventoryAffiliated> receivingInventory;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var giver = ((IInventoryAffiliated)givingInventory.actionField.GetFieldValue());
			var receiver = ((IInventoryAffiliated)receivingInventory.actionField.GetFieldValue());
			var item = itemField.GetTypedFieldValue();
			if(giver.Inventory.Items.Contains(item))
			{
				giver.Inventory.Items.Remove(item);
				receiver.Inventory.Items.Add(item);
			}
		}
	}
}