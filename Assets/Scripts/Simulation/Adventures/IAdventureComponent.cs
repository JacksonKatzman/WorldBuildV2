using System.Collections.Generic;

namespace Game.Simulation
{
	public interface IAdventureComponent
	{
		public bool Completed { get; set; }
		public int ComponentID { get; set; }
		public void UpdateRetrieverIds(int oldID, int newID);
	}
}
