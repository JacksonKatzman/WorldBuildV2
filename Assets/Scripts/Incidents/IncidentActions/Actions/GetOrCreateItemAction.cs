using Game.Generators.Items;
using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class GetOrCreateItemAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Item> item;

		[ValueDropdown("GetFilteredTypeList")]
		public Type itemType;
		public ActionResultField<Item> resultItem;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			if (item.GetTypedFieldValue() == null)
			{
				var newItem = (Item)Activator.CreateInstance(itemType);
				SimulationManager.Instance.world.AddContext(newItem);
				resultItem.SetValue(newItem);
			}
			else
			{
				resultItem.SetValue(item.GetTypedFieldValue());
			}
		}

		private IEnumerable<Type> GetFilteredTypeList()
		{
			var q = typeof(Item).Assembly.GetTypes()
				.Where(x => !x.IsAbstract)                                          // Excludes BaseClass
				.Where(x => !x.IsGenericTypeDefinition)                             // Excludes Generics
				.Where(x => typeof(Item).IsAssignableFrom(x));

			return q;
		}
	}
}