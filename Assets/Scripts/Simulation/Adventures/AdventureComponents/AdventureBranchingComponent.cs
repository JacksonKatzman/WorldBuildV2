using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Game.Simulation
{
	public class AdventureBranchingComponent : AdventureComponent
	{
		[Title("Branching Action Paths"), ListDrawerSettings(CustomAddFunction = "AddPath")]
		public ObservableCollection<AdventurePathComponent> paths = new ObservableCollection<AdventurePathComponent>();

		private void AddPath()
		{
			paths.Add(new AdventurePathComponent());
		}
	}
}
