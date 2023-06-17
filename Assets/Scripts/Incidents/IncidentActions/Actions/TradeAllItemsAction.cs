namespace Game.Incidents
{
	public class TradeAllItemsAction : GenericIncidentAction
	{
		public InterfacedIncidentActionFieldContainer<IInventoryAffiliated> givingInventory;
		public InterfacedIncidentActionFieldContainer<IInventoryAffiliated> receivingInventory;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var giver = ((IInventoryAffiliated)givingInventory.actionField.GetFieldValue());
			var receiver = ((IInventoryAffiliated)receivingInventory.actionField.GetFieldValue());
			receiver.CurrentInventory.Items.AddRange(giver.CurrentInventory.Items);
			giver.CurrentInventory.Items.Clear();
		}
	}
}