using Game.Simulation;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

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

			AddKeywordLinks(text);
		}

		public override void OnPointerClick(PointerEventData eventData)
		{
			HandleClicks(text);
		}

		protected override void ToggleElements()
		{
			OutputLogger.Log("TOGGLE ELEMENTS");
			text.color = Completed ? SwapColorAlpha(text.color, FADED_ALPHA) : SwapColorAlpha(text.color, FULL_ALPHA);
		}

		protected void Update()
		{
			if(hovered)
			{
				HandleTooltips(text);
			}
		}
	}
}
