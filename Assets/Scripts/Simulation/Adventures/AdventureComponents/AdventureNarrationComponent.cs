using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureNarrationComponent : AdventureComponent, IAdventureTextComponent
	{
		[Title("Narration Text")]
		AdventureComponentTextField textField = new AdventureComponentTextField();
		public string Text => textField.text;
	}
}
