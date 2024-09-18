using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Game.Simulation
{
	public class AdventurePathComponent : AdventureComponent
	{
		public string pathTitle;
		public ObservableCollection<IAdventureComponent> components = new ObservableCollection<IAdventureComponent>();
	}
}
