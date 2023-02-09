using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
	[HideReferenceObjectPicker]
	public class AdventureEncounterAct
	{
		public string actTitle;
		[TextArea(15, 20), PropertyOrder(0)]
		public string actNotes;
		[TextArea(15, 20), PropertyOrder(0)]
		public string actApproach;
		public AdventureEncounterAct()
		{
			paths = new List<AdventureEncounterPath>();
		}

		[ListDrawerSettings(CustomAddFunction = "AddPath")]
		public List<AdventureEncounterPath> paths;

		private void AddPath()
		{
			paths.Add(new AdventureEncounterPath());
		}
	}

	//major encounters vs minor encounters?
	//dungeon encounters which are made up of majors and minors
	//adventure is some number of minors and a 1-3 majors?
}
