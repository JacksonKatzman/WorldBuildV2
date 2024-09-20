using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureTextComponent : AdventureComponent, IAdventureTextComponent
	{
		public AdventureComponentTextField textField = new AdventureComponentTextField();

		public string Text => textField.Text;
		public AdventureTextComponent() { }
		public AdventureTextComponent(string text)
        {
			textField.SetSingleText(text);
        }
	}
}
