using Game.Simulation;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.GUI.Adventures
{
	abstract public class AdventureSingleTextUIComponent<T> : AdventureUIComponent<T> where T : IAdventureComponent
	{
		public TMP_Text text;

		override protected List<TMP_Text> AssociatedTexts => new List<TMP_Text>() { text };

		public override void BuildUIComponents(T component)
		{
			var textComponent = component as IAdventureTextComponent;

			text.text = textComponent.Text;
		}

		protected void Update()
		{
			if (hovered)
			{
				HandleTooltips(text);
			}
		}
	}
}
