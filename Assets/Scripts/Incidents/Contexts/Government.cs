using Game.Simulation;

namespace Game.Incidents
{
	public class Government : IFactionAffiliated
	{
		public Faction AffiliatedFaction { get; set; }
		public Person Leader { get; set; }

		public Government() { }
		public Government(Faction faction)
		{
			AffiliatedFaction = faction;
			EventManager.Instance.AddEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
		}

		public void OnRemoveContextEvent(RemoveContextEvent gameEvent)
		{
			if(gameEvent.context == Leader)
			{
				SelectNewLeader(Leader.Race);
			}
		}

		public void SelectNewLeader(Race majorityRace)
		{
			Leader = new Person(35, Enums.Gender.ANY, majorityRace, AffiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10);
			SimulationManager.Instance.world.AddContext(Leader);
		}
	}
}