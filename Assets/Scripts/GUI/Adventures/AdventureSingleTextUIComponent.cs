using Game.Simulation;
using System.Collections.Generic;
using TMPro;

namespace Game.GUI.Wiki
{
	abstract public class AdventureSingleTextUIComponent : AdventureUIComponent
	{
		public TMP_Text text;

		public override void BuildUIComponents(IAdventureComponent component)
		{
			var textComponent = component as AdventureTextComponent;

			text.text = textComponent.text;
		}

		public override void ReplaceTextPlaceholders(List<IAdventureContextCriteria> contexts)
		{
			foreach (var context in contexts)
			{
				var currentText = text.text;
				context.ReplaceTextPlaceholders(ref currentText);
				text.text = currentText;
			}
		}

		protected override void ToggleElements()
		{
			OutputLogger.Log("TOGGLE ELEMENTS");
			text.color = Completed ? SwapColorAlpha(text.color, FADED_ALPHA) : SwapColorAlpha(text.color, FULL_ALPHA);
		}
	}
}
