using System.Collections.Generic;

namespace Game.Simulation
{
	public interface IAdventureComponent
	{
		public bool Completed { get; set; }
		public int ComponentID { get; set; }
		public void UpdateStuff(int oldID, int newID);
		public void UpdateComponentID(ref int nextID, List<int> removedIds = null);
		public void UpdateContextIDs(List<int> removedIds = null);
		public List<int> GetRemovedIds();
	}
}
