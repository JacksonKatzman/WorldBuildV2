using Game.Simulation;
using System.Collections.Generic;
using TMPro;

namespace Game.GUI.Wiki
{
	public class AdventureTextTitlePairUIComponent : AdventureUIComponent
	{
		public TMP_Text title;
		public TMP_Text text;

		public override void BuildUIComponents(IAdventureComponent component)
		{
			var textComponent = component as AdventureTextTitlePairComponent;

			title.text = textComponent.title;
			text.text = textComponent.text;
		}

		public override void ReplaceTextPlaceholders(List<IAdventureContextCriteria> contexts)
		{
			foreach (var context in contexts)
			{
				var currentText = title.text;
				context.ReplaceTextPlaceholders(ref currentText);
				title.text = currentText;
			}
			foreach (var context in contexts)
			{
				var currentText = text.text;
				context.ReplaceTextPlaceholders(ref currentText);
				text.text = currentText;
			}

			AdventureGuide.Instance.CreateTableOfContentsEntry(ComponentID, title.text);
		}

		protected override void ToggleElements()
		{
			OutputLogger.Log("TOGGLE ELEMENTS");
			title.color = Completed ? SwapColorAlpha(title.color, FADED_ALPHA) : SwapColorAlpha(title.color, FULL_ALPHA);
			text.color = Completed ? SwapColorAlpha(text.color, FADED_ALPHA) : SwapColorAlpha(text.color, FULL_ALPHA);
		}
	}
}
