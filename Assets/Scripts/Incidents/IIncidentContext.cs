using Game.Factions;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;

namespace Game.Incidents
{
	public interface IIncidentContext
	{
		IIncidentContextProvider Provider { get; }
		Type ContextType { get; }
		int NumIncidents { get; }
		int ParentID { get; }
	}

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