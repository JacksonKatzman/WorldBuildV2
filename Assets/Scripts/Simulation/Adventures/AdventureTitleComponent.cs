using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Game.Simulation
{
	public class AdventureTitleComponent : AdventureComponent, IAdventureTextComponent
	{ 
		[Title("Section Title")]
		public string title;

		public string Text => title;

		override public void UpdateContextIDs(List<int> removedIds = null)
		{
			title = EncounterEditorWindow.UpdateInTextIDs(title, removedIds);
		}
	}

	public interface IAdventureTextComponent
	{
		public string Text { get; }
	}
}
