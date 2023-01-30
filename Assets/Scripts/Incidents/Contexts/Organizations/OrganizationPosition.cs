using Game.Enums;
using Game.Generators.Names;
using Game.Simulation;
using Game.Utilities;
using System.Collections.Generic;

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
			var previousOfficial = official;
			official = SimRandom.RandomRange(0, 5) < 4 && previousOfficial != null ? official.CreateChild(true) : new Person(35, Enums.Gender.ANY, majorityRace, affiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10, true);
			official.OfficialPosition = this;
			SimulationManager.Instance.world?.AddContext(official);
			var report = previousOfficial == null ? "{0} takes power." : "{0} succeeds {1}.";
			IncidentService.Instance.ReportStaticIncident(report, new List<IIncidentContext>() { official, previousOfficial });
		}
	}
}