
using Game.Incidents;

namespace Game.Simulation
{
	public class AffiliatedOrganizationChangedEvent : ISimulationEvent
	{
        public ISentient affiliate;
        public IOrganizationPosition newPosition;

		public AffiliatedOrganizationChangedEvent(ISentient affiliate, IOrganizationPosition newPosition)
		{
			this.affiliate = affiliate;
			this.newPosition = newPosition;
		}
	}
}
