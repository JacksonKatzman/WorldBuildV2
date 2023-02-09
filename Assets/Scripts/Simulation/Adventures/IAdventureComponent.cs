using System.Collections.Generic;

namespace Game.Simulation
{
	public interface IAdventureComponent
	{
		public bool Completed { get; set; }
		public int ComponentID { get; set; }
		public void UpdateComponentID(ref int nextID, List<int> removedIds = null);
		public List<int> GetRemovedIds();
	}
}
