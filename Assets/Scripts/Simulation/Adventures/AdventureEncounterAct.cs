using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Game.Simulation
{
	[HideReferenceObjectPicker]
	public class AdventureEncounterAct
	{
		public string actTitle;
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
}
