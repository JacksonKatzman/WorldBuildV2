using System;

namespace Game.Incidents
{
	public abstract class InertIncidentContext : IIncidentContext
	{
		abstract public Type ContextType { get; }

		public int NumIncidents => 0;

		public int ParentID => -1;

		public void DeployContext()
		{

		}

		public void UpdateContext()
		{

		}
	}
}