using Game.Enums;
using Game.Generators.Names;
using Game.Simulation;

namespace Game.Incidents
{
	public class OrganizationPosition
	{
		public Person official;
		public TitlePair titlePair;
		public OrganizationType organizationType;
		//responsibilities

		public OrganizationPosition() { }
		public OrganizationPosition(OrganizationType organizationType)
		{
			this.organizationType = organizationType;
		}
		public void SelectNewOfficial(Faction affiliatedFaction, Race majorityRace)
		{
			official = new Person(35, Enums.Gender.ANY, majorityRace, affiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10);
			SimulationManager.Instance.world?.AddContext(official);
		}
	}
}