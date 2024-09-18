using Game.Generators.Items;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureTreasureComponent : AdventureComponent
	{
		public List<AdventureTreasure> treasures;
		public List<ItemValue> currency;

		[PropertyOrder(0)]
		public AdventureComponentTextField description = new AdventureComponentTextField();

		public AdventureTreasureComponent()
		{
			treasures = new List<AdventureTreasure>();
			currency = new List<ItemValue>();
		}
	}

	public class AdventureTreasure
	{
		[ValueDropdown("GetTreasureIDs")]
		public int treasureID;
		public int amount;
#if UNITY_EDITOR
		private IEnumerable<int> GetTreasureIDs()
		{
			var treasureRetrievers = AdventureEncounterObject.Current.contextCriterium.Where(x => x.GetType() == typeof(TreasureRetriever)).Select(x => x.RetrieverID).ToList();
			return treasureRetrievers;
		}
#endif
	}
}
