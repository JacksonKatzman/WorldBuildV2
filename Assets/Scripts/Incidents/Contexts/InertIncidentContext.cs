using Game.Simulation;
using System;
using System.Data;

namespace Game.Incidents
{
	public abstract class InertIncidentContext : IncidentContext
	{
		override public int NumIncidents => 0;

		override public void DeployContext()
		{

		}

		override public DataTable GetDataTable()
		{
			return new DataTable();
		}

		override public void UpdateContext()
		{

		}

		override public void Die()
		{
			EventManager.Instance.Dispatch(new RemoveContextEvent(this));
		}

		override public void UpdateHistoricalData()
		{

		}

		override public void LoadContextProperties() { }
	}
}