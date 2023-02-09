using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Game.Simulation
{
	public class AdventurePathComponent : AdventureComponent
	{
		public ObservableCollection<IAdventureComponent> components;

		public AdventurePathComponent()
		{
			components = new ObservableCollection<IAdventureComponent>();
			components.CollectionChanged += EncounterEditorWindow.UpdateIDs;
		}

		public override void UpdateComponentID(ref int nextID, List<int> removedIds = null)
		{
			base.UpdateComponentID(ref nextID, removedIds);
			foreach (var component in components)
			{
				component.UpdateComponentID(ref nextID, removedIds);
			}
		}
	}
}
