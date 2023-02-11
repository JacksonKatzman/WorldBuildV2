using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Game.Simulation
{
	public class AdventureBranchingComponent : AdventureComponent
	{
		[Title("Branching Action Paths"), ListDrawerSettings(CustomAddFunction = "AddPath")]
		public ObservableCollection<AdventurePathComponent> paths;

		public AdventureBranchingComponent()
		{
			paths = new ObservableCollection<AdventurePathComponent>();
			paths.CollectionChanged += EncounterEditorWindow.UpdateComponentIDs;
		}

		public override void UpdateComponentID(ref int nextID, List<int> removedIds = null)
		{
			base.UpdateComponentID(ref nextID, removedIds);
			foreach (var path in paths)
			{
				path.UpdateComponentID(ref nextID, removedIds);
			}
		}

		private void AddPath()
		{
			paths.Add(new AdventurePathComponent());
		}
	}
}
