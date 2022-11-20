namespace Game.Incidents
{
	public class TradeItemsAction : GenericIncidentAction
	{
		public InterfacedIncidentActionFieldContainer<IInventoryAffiliated> givingInventory;
		public InterfacedIncidentActionFieldContainer<IInventoryAffiliated> receivingInventory;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var giver = ((IInventoryAffiliated)givingInventory.actionField.GetFieldValue());
			var receiver = ((IInventoryAffiliated)receivingInventory.actionField.GetFieldValue());
			receiver.Inventory.AddRange(giver.Inventory);
			giver.Inventory.Clear();
		}
	}
}