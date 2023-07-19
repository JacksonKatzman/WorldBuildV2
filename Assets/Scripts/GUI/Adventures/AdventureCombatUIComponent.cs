using Game.Simulation;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;

namespace Game.GUI.Adventures
{
	public class AdventureCombatUIComponent : AdventureUIComponent<AdventureCombatComponent>
	{
		public TMP_Text participantsText;
		public TMP_Text descriptionText;

		protected override List<TMP_Text> AssociatedTexts => new List<TMP_Text>() { participantsText, descriptionText };

		public override void BuildUIComponents(AdventureCombatComponent component)
		{
			descriptionText.text = component.description;

			foreach (var participant in component.combatParticipants)
			{
				var id = "{" + participant.combatantID + "}";
				var entry = string.Format("<indent=1em>\u2022 {0} {1}. </indent> \n", participant.amount, id);
				participantsText.text += entry;
			}
		}
	}
}
