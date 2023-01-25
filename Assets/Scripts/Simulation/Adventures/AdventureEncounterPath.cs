using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Simulation
{
	[HideReferenceObjectPicker]
	public class AdventureEncounterPath
	{
		public string pathTitle;
		[TextArea(10,15), FoldoutGroup("Summary")]
		public string pathSummary;
		[TextArea(15,20), FoldoutGroup("Full Description")]
		public string pathInformation;
	}
}
