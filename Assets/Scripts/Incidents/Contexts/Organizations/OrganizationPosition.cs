using Game.Enums;
using Game.Generators.Names;
using Game.Simulation;
using Game.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class OrganizationPosition
	{
		public Character official;
		public TitlePair titlePair;
		public OrganizationType organizationType;
		public bool hereditary = true;
		//responsibilities

		public OrganizationPosition() { }
		public OrganizationPosition(OrganizationType organizationType)
		{
			this.organizationType = organizationType;
		}
		public void SelectNewOfficial(Organization org, Faction affiliatedFaction, Race majorityRace)
		{
			var found = false;
			var previousOfficial = official;
			if (previousOfficial != null && hereditary)
			{
				var viableChildren = previousOfficial.Children.Where(x => x.MajorCharacter && ContextDictionaryProvider.CurrentContexts[typeof(Character)].Contains(x)).ToList();
				var viableChild = viableChildren.Count > 0 ? viableChildren.First() : null;
				if (viableChild != null)
				{
					official = viableChild;
					found = true;
				}
			}

			if (!found)
			{
				if (hereditary)
				{
					official = SimRandom.RandomRange(0, 5) < 4 && previousOfficial != null ? official.CreateChild(true) : new Character(35, Enums.Gender.ANY, majorityRace, affiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10, true);
					official.GenerateFamily(true, true);
				}
				else
				{
					official = SimRandom.RandomRange(0, 5) < 1 && previousOfficial != null ? official.CreateChild(true) : new Character(35, Enums.Gender.ANY, majorityRace, affiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10, true);
					official.GenerateFamily(true, true);
				}

				EventManager.Instance.Dispatch(new AddContextEvent(official, false));
			}

			official.AffiliatedOrganization = org;
			var report = previousOfficial == null ? "{0} takes power." : "{0} succeeds {1}.";
			IncidentService.Instance.ReportStaticIncident(report, new List<IIncidentContext>() { official, previousOfficial }, true);
			/*
			var previousOfficial = official;
			if(previousOfficial != null && hereditary)
			{
				var viableChild = previousOfficial.Children.First(x => x.MajorCharacter && ContextDictionaryProvider.CurrentContexts[typeof(Character)].Contains(x));
				if(viableChild != null)
				{
					official = viableChild;
				}
			}
			else
			{
				official = SimRandom.RandomRange(0, 5) < 4 && previousOfficial != null ? official.CreateChild(true) : new Character(35, Enums.Gender.ANY, majorityRace, affiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10, true);
				official.GenerateFamily(true, true);
				EventManager.Instance.Dispatch(new AddContextEvent(official, false));
			}
			
			official.AffiliatedOrganization = org;
			var report = previousOfficial == null ? "{0} takes power." : "{0} succeeds {1}.";
			IncidentService.Instance.ReportStaticIncident(report, new List<IIncidentContext>() { official, previousOfficial }, true);
			*/
		}
	}
}