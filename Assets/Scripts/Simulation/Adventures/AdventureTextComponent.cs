using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureTextComponent : AdventureComponent, IAdventureTextComponent
	{
		[TextArea(15, 20), PropertyOrder(0)]
		public string text;

		public string Text => text;

		override public void UpdateContextIDs(List<int> removedIds = null)
		{
#if UNITY_EDITOR
			text = EncounterEditorWindow.UpdateInTextIDs(text, removedIds);
#endif
		}
	}
}
