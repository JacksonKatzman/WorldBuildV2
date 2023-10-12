using Game.Generators.Items;
using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class GetOrCreateItemAction : GetOrCreateAction<Item>
	{
		public InterfacedIncidentActionFieldContainer<IInventoryAffiliated> inventoryToAddTo;
		[ValueDropdown("GetFilteredTypeList"), ShowIf("@this.allowCreate")]
		public Type itemType;
		[ShowIf("@this.allowCreate")]
		public bool createdByCharacter;
		[ShowIf("@this.createdByCharacter")]
		public ContextualIncidentActionField<Character> creator;

		protected override Item MakeNew()
		{
			var newItem = (Item)Activator.CreateInstance(itemType);
			newItem.RollStats(100);
			//((IInventoryAffiliated)inventoryToAddTo.actionField.GetFieldValue()).CurrentInventory.Items.Add(newItem);

			if(createdByCharacter)
			{
				newItem.Name = creator.GetTypedFieldValue().AffiliatedFaction.namingTheme.GenerateItemName(newItem, creator.GetTypedFieldValue());
			}
			else
			{
				newItem.Name = "Unnamed Item";
			}

			return newItem;
		}

		override protected void Complete()
		{
			var inventory = (IInventoryAffiliated)inventoryToAddTo.actionField.GetFieldValue();
			var item = actionField.GetTypedFieldValue();

			if (madeNew)
			{
				ContextDictionaryProvider.AddContext(actionField.GetTypedFieldValue());
				inventory.CurrentInventory.Items.Add(item);
			}
			else if(inventory != null)
			{
				inventory.CurrentInventory.Items.Add(item);
			}
		}

		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			var check = createdByCharacter ? inventoryToAddTo.actionField.CalculateField(context) && creator.CalculateField(context) : inventoryToAddTo.actionField.CalculateField(context);
			return check;
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