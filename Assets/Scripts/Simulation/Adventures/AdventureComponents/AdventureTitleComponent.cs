using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Game.Simulation
{
	public class AdventureTitleComponent : AdventureComponent, IAdventureTextComponent
	{ 
		[Title("Section Title")]
		public AdventureComponentTextField title = new AdventureComponentTextField();
		public string Text => title.text;

		public AdventureTitleComponent() { }
		public AdventureTitleComponent(string title)
        {
			this.title.text = title;
        }
	}
}
