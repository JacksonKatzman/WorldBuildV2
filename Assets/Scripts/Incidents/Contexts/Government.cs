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
			Leader = new Person(35, Enums.Gender.ANY, null, AffiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10);
			Leader.SetOnDeathAction(SelectNewLeader);
			SimulationManager.Instance.world.AddContext(Leader);
		}
	}
}