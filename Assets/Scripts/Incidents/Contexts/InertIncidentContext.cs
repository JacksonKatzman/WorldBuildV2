using System;
using System.Data;

namespace Game.Incidents
{
	public abstract class InertIncidentContext : IIncidentContext
	{
		abstract public Type ContextType { get; }

		public int NumIncidents => 0;
		public int ID { get; set; }

		public int ParentID => -1;

		public void DeployContext()
		{

		}

		public DataTable GetDataTable()
		{
			return new DataTable();
		}

		public void UpdateContext()
		{

		}

		public void UpdateHistoricalData()
		{

		}
	}
}