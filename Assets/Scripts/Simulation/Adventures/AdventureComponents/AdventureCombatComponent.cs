using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureCombatComponent : AdventureComponent
	{
		public List<CombatParticipant> combatParticipants;

		[TextArea(15, 20), PropertyOrder(0)]
		public string description;

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
			var ids = EncounterEditorWindow.contextCriterium.Where(x => x.GetType() == typeof(MonsterCriteria)).Select(x => x.CriteriaID);
			return ids;
		}
#endif
	}
}
