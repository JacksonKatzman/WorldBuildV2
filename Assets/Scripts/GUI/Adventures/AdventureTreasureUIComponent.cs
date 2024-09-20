using Game.Simulation;
using System.Collections.Generic;
using TMPro;

namespace Game.GUI.Adventures
{
	public class AdventureTreasureUIComponent : AdventureUIComponent<AdventureTreasureComponent>
	{
		public TMP_Text treasureListText;
		public TMP_Text descriptionText;

		protected override List<TMP_Text> AssociatedTexts => new List<TMP_Text>() { treasureListText, descriptionText };

		public override void BuildUIComponents(AdventureTreasureComponent component)
		{
			treasureListText.text = string.Empty;
			descriptionText.text = component.description.Text;

			foreach (var treasure in component.treasures)
			{
				var id = "{" + treasure.treasureID + "}";
				var entry = string.Format("<indent=1em><b>\u2022 {0} {1}. </b></indent> \n", treasure.amount, id);
				treasureListText.text += entry;
			}

			var coins = string.Empty;
			foreach(var value in component.currency)
			{
				coins += string.Format("{0} {1}, ", value.amount, value.coinType.ToString());
			}

			if(coins != string.Empty)
			{
				var entry = string.Format("<indent=1em><b>\u2022 {0}. </b></indent>", coins);
				treasureListText.text += entry;
			}
		}
	}
}
