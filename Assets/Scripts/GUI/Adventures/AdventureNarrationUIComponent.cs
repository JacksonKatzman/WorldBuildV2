using Game.Simulation;
using Sirenix.OdinInspector;
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
			//throw new System.NotImplementedException();
		}

		protected override void ToggleElements()
		{
			text.alpha = Completed ? 100.0f : 255.0f;
		}
	}
}
