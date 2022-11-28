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
		[ValueDropdown("GetFilteredTypeList"), ShowIf("@this.allowCreate")]
		public Type itemType;

		protected override Item MakeNew()
		{
			var newItem = (Item)Activator.CreateInstance(itemType);

			return newItem;
			//SimulationManager.Instance.world.AddContext(newItem);
			//result.SetValue(newItem);
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