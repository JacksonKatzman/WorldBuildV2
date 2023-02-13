using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Game.Simulation
{
	public class AdventureTitleComponent : AdventureComponent
	{
		[Title("Section Title")]
		public string title;

		override public void UpdateContextIDs(List<int> removedIds = null)
		{
			title = EncounterEditorWindow.UpdateInTextIDs(title, removedIds);
		}
	}
}
