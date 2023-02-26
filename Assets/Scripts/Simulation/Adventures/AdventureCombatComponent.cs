using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

namespace Game.Simulation
{
	public class AdventureCombatComponent : AdventureComponent
	{
		public List<CombatParticipant> combatParticipants;

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

		private IEnumerable<int> GetCombatantIDs()
		{
			var ids = EncounterEditorWindow.contextCriterium.Where(x => x.GetType() == typeof(MonsterCriteria)).Select(x => x.ContextID);
			return ids;
		}
	}
}
