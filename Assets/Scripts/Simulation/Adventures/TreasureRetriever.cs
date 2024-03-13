using Game.Data;
using Game.Generators.Items;
using Game.Incidents;
using Game.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Simulation
{
	public class TreasureRetriever : AdventureContextRetriever<Item>
	{
		[ShowIf("@this.IsHistorical == false")]
		public bool premade;

		public ItemType data;

		[ValueDropdown("GetFilteredTypeList"), ShowIf("@this.IsHistorical == true")]
		public Type type;
		[ShowIf("@this.premade == false")]
		public IntegerRange pointRange = new IntegerRange();
		public override Dictionary<string, Func<Item, int, string>> Replacements => replacements;
		private static readonly Dictionary<string, Func<Item, int, string>> replacements = new Dictionary<string, Func<Item, int, string>>
		{
			{"{##}", (item, criteriaID) => string.Format("<i><link=\"{0}\">{1}</link></i>", criteriaID, item.Name) }
		};

		public override Item RetrieveContext()
		{
			if (historical)
			{
				//grab an item from the world
				var possibilities = SimulationManager.Instance.AllContexts.GetContextsByType<Item>()
					.Where(x => (x.GetType() == type) && (x.Points >= pointRange.min && x.Points <= pointRange.max)).ToList();
				if (possibilities.Count > 0)
				{
					return SimRandom.RandomEntryFromList(possibilities);
				}
				else
				{
					return MakeNew();
				}
			}
			else
			{
				return MakeNew();
			}
		}

		public override void SpawnPopup()
		{
			throw new NotImplementedException();
		}

		private Item MakeNew()
		{
			var newItem = Item.Create(data);
			newItem.Name = data.Name;
			if (!premade)
			{
				newItem.RollStats(pointRange.Value);
			}
			return newItem;
		}

		private IEnumerable<Type> GetFilteredTypeList()
		{
			return IncidentHelpers.GetFilteredTypeList(typeof(Item));
		}
	}
}
