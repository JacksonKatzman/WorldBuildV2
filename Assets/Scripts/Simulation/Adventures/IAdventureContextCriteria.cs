using Game.Incidents;
using System;

namespace Game.Simulation
{
	public interface IAdventureContextCriteria 
	{
		public Type ContextType { get; }
		public string ContextID { get; set; }
		public IIncidentContext Context { get; set; }
		public bool IsHistorical { get; }
	}
}
