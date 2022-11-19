using System;
using System.Data;

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
		void UpdateHistoricalData();
		DataTable GetDataTable();
	}
}