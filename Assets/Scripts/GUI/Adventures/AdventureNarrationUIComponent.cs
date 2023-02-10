using Game.Simulation;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.GUI.Wiki
{
	public class AdventureNarrationUIComponent : AdventureUIComponent
	{
		[Title("Narration Text")]
		public TMP_Text text;

		public override void BuildUIComponents(IAdventureComponent component)
		{
			var typedComponent = component as AdventureNarrationComponent;
			text.text = typedComponent.text;
		}

		protected override void ToggleElements()
		{
			text.alpha = Completed ? 100.0f : 255.0f;
		}
	}
}
