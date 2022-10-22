using System;

namespace Game.Incidents
{
	[Serializable]
	public class FactionContext : IIncidentContext
	{
		public IIncidentContextProvider Provider { get; set; }
		public Type ContextType => typeof(FactionContext);
		public int NumIncidents { get; set; }
		public int ParentID => -1;
		public int Population { get; set; }
		public float GooPercentage { get; set; }
		public bool IsFun { get; set; }
	}
}