using Game.Incidents;
using System;

namespace Game.Factions
{
	public class Faction : IIncidentContextProvider
	{
		public FactionContext context;
		public Type ContextType => typeof(FactionContext);

		public Faction()
		{
			context = new FactionContext();
		}

		public void DeployContext()
		{
			IncidentService.Instance.PerformIncidents(this);
		}

		public IIncidentContext GetContext()
		{
			return context;
		}

		public void UpdateContext()
		{
			throw new System.NotImplementedException();
		}
	}
}