using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureNarrationComponent : AdventureComponent
	{
		[TextArea(15, 20), PropertyOrder(0), Title("Narration Text")]
		public string text;

		override public void UpdateContextIDs(List<int> removedIds = null)
		{
			text = EncounterEditorWindow.UpdateInTextIDs(text, removedIds);
		}
	}
}
