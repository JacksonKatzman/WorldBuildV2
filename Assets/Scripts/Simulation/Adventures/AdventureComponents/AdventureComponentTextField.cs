using Sirenix.OdinInspector;
using System;
using UnityEngine;

namespace Game.Simulation
{
	[Serializable, HideReferenceObjectPicker]
    public class AdventureComponentTextField
    {
		[TextArea(1, 20), PropertyOrder(0)]
		public string text;

		public void UpdateIDs(int oldID, int newID)
        {
			text = text.Replace($":{oldID}}}", $":{newID}}}");
		}
	}
}
