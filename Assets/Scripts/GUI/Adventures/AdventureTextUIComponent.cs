using Game.Simulation;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

namespace Game.GUI.Wiki
{
	public class AdventureTextUIComponent : AdventureUIComponent
	{
		[Title("Descriptive/Background Text")]
		public TMP_Text title;

		public TMP_Text text;

		public override void BuildUIComponents(IAdventureComponent component)
		{
			var textComponent = component as AdventureTextComponent;
			if(textComponent.title != null && textComponent.title != string.Empty)
			{
				title.text = textComponent.title;
			}
			else
			{
				title.gameObject.SetActive(false);
			}

			text.text = textComponent.text;
		}

		protected override void ToggleElements()
		{
			OutputLogger.Log("TOGGLE ELEMENTS");
			title.color = Completed ? SwapColorAlpha(title.color, FADED_ALPHA) : SwapColorAlpha(title.color, FULL_ALPHA);
			text.color = Completed ? SwapColorAlpha(text.color, FADED_ALPHA) : SwapColorAlpha(text.color, FULL_ALPHA);
		}
	}
}
