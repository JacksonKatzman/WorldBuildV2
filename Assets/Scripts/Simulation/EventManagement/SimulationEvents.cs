using Game.Incidents;
using System;

namespace Game.Simulation
{
    public class AddContextEvent : ISimulationEvent
    {
        public IIncidentContext context;
        public Type contextType;
        public bool immediate;

        public AddContextEvent(IIncidentContext context, Type type = null, bool immediate = false)
        {
            this.context = context;
            this.contextType = type == null ? context.GetType() : type;
            this.immediate = immediate;
        }

        public AddContextEvent(IIncidentContext context, bool immediate = false)
        {
            this.context = context;
            this.contextType = context.GetType();
            this.immediate = immediate;
        }
    }

    public class RemoveContextEvent : ISimulationEvent
    {
        public IIncidentContext context;
        public Type contextType;

        public RemoveContextEvent(IIncidentContext context, Type type = null)
        {
            this.context = context;
            this.contextType = type == null ? context.GetType() : type;
        }
    }

    public class AffiliatedFactionChangedEvent : ISimulationEvent
    {
        public IFactionAffiliated affiliate;
        public IFactionAffiliated newFaction;

        public AffiliatedFactionChangedEvent(IFactionAffiliated affiliate, IFactionAffiliated newFaction)
        {
            this.affiliate = affiliate;
            this.newFaction = newFaction;
        }
    }

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

    public class WarDeclaredEvent : ISimulationEvent
    {
        public IFactionAffiliated attacker;
        public IFactionAffiliated defender;

        public WarDeclaredEvent(IFactionAffiliated attacker, IFactionAffiliated defender)
        {
            this.attacker = attacker;
            this.defender = defender;
        }
    }

    public class PeaceDeclaredEvent : ISimulationEvent
    {
        public IFactionAffiliated declarer;
        public IFactionAffiliated accepter;

        public PeaceDeclaredEvent(IFactionAffiliated declarer, IFactionAffiliated accepter)
        {
            this.declarer = declarer;
            this.accepter = accepter;
        }
    }

    public class TerritoryChangedControlEvent : ISimulationEvent
	{
        public IFactionAffiliated territoryLoser;
        public IFactionAffiliated territoryGainer;
        public ILocationAffiliated location;

		public TerritoryChangedControlEvent(IFactionAffiliated territoryLoser, IFactionAffiliated territoryGainer, ILocationAffiliated location)
		{
			this.territoryLoser = territoryLoser;
			this.territoryGainer = territoryGainer;
			this.location = location;
		}
	}

    public class CityChangedControlEvent : ISimulationEvent
	{
        public IFactionAffiliated cityLoser;
        public IFactionAffiliated cityGainer;
        public City city;

		public CityChangedControlEvent(IFactionAffiliated cityLoser, IFactionAffiliated cityGainer, City city)
		{
			this.cityLoser = cityLoser;
			this.cityGainer = cityGainer;
			this.city = city;
		}
	}
}
