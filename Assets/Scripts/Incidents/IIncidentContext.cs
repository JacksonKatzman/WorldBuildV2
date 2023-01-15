using System;
using System.Data;

namespace Game.Incidents
{
	public interface IIncidentContext
	{
		Type ContextType { get; }
		int NumIncidents { get; }
		string Name { get; set; }
		int ID { get; set; }
		int ParentID { get; }
		void UpdateContext();
		void DeployContext();
		void Die();
		void UpdateHistoricalData();
		DataTable GetDataTable();
	}
}