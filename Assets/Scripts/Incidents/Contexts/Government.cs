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
		}

		public void SelectNewLeader()
		{
			var leaderRace = Leader == null ? new Race() : Leader.Race;
			if(Leader == null)
			{
				SimulationManager.Instance.world.AddContext(leaderRace);
			}
			Leader = new Person(35, Enums.Gender.ANY, leaderRace, AffiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10);
			Leader.SetOnDeathAction(SelectNewLeader);
			SimulationManager.Instance.world.AddContext(Leader);
		}
	}
}