using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureTextComponent : AdventureComponent
	{
		[TextArea(15, 20), PropertyOrder(0)]
		public string text;

		override public void UpdateContextIDs(List<int> removedIds = null)
		{
			text = EncounterEditorWindow.UpdateInTextIDs(text, removedIds);
		}
	}
}
