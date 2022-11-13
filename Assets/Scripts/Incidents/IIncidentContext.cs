using System;

namespace Game.Incidents
{
	public interface IIncidentContext
	{
		Type ContextType { get; }
		int NumIncidents { get; }
		int ID { get; set; }
		int ParentID { get; }
		void UpdateContext();
		void DeployContext();
	}
}