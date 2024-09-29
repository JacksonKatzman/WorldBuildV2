using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Simulation
{
    public class AdventureCombatComponent : AdventureComponent
	{
		public List<CombatParticipant> combatParticipants;

		[PropertyOrder(0)]
		public AdventureComponentTextField description = new AdventureComponentTextField();

		public AdventureCombatComponent()
		{
			combatParticipants = new List<CombatParticipant>();
		}
	}

	[HideReferenceObjectPicker]
	public class CombatParticipant
	{
		[ValueDropdown("GetCombatantIDs")]
		public int combatantID;
		public int amount;

#if UNITY_EDITOR
		private IEnumerable<int> GetCombatantIDs()
		{
			var monsterRetrievers = AdventureEncounterObject.Current.contextCriterium.Where(x => x.GetType() == typeof(MonsterRetriever)).Select(x => x.RetrieverID).ToList();
			return monsterRetrievers;
		}
#endif
	}
}
