using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureTextTitlePairComponent : AdventureTitleComponent
	{
		[TextArea(15, 20), PropertyOrder(0)]
		public string text;

		override public void UpdateContextIDs(List<int> removedIds = null)
		{
			title = EncounterEditorWindow.UpdateInTextIDs(title, removedIds);
			text = EncounterEditorWindow.UpdateInTextIDs(text, removedIds);
		}
	}
}
