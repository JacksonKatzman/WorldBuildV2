using Game.Incidents;
using System;
using System.Collections.Generic;

namespace Game.Simulation
{
	public interface IAdventureContextCriteria 
	{
		public Type ContextType { get; }
		public int ContextID { get; set; }
		public IIncidentContext Context { get; set; }
		public bool IsHistorical { get; }

		public void ReplaceTextPlaceholders(ref string text);
	}
}
