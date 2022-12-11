using System;

namespace Game.Incidents
{
	public class InventoryEvaluator : ContextEvaluator<Inventory, IInventoryAffiliated>
	{
		public InventoryEvaluator() : base() { }
		public InventoryEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }

		protected override Inventory GetContext(IIncidentContext context)
		{
			if ((typeof(IInventoryAffiliated)).IsAssignableFrom(context.ContextType))
			{
				return ((IInventoryAffiliated)context).Inventory;
			}
			else
			{
				return null;
			}
		}
	}
}