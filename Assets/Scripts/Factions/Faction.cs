using Game.Incidents;

namespace Game.Factions
{
	public class Faction : IIncidentContextProvider
	{
		public FactionContext context;

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