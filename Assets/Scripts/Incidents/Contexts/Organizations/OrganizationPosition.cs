using Game.Enums;
using Game.Generators.Names;
using Game.Simulation;
using Game.Utilities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public class OrganizationPosition : AbstractOrganizationComponent, IOrganizationPosition
	{
		[HideInInspector]
		public ISentient official;
		[HideInInspector]
		public OrganizationType organizationType;
		public TitlePair titlePair;
		public bool hereditary = true;
		public Gender gender;
		public int maxPositions;
		//responsibilities

		public OrganizationPosition() { }
		public OrganizationPosition(OrganizationType organizationType)
		{
			this.organizationType = organizationType;
		}

		public OrganizationPosition(OrganizationPosition template)
		{
			titlePair = template.titlePair;
			hereditary = template.hereditary;
			gender = template.gender;
		}

		public override bool Contains(ISentient sentient, out IOrganizationPosition pos)
		{
			if(sentient == official)
			{
				pos = this;
				return true;
			}
			else
			{
				pos = null;
				return false;
			}
		}

		public override void GetPositionCount(ref int total, bool careAboutFilled)
		{
			if((careAboutFilled && official != null) || !careAboutFilled)
			{
				total += 1;
			}
		}

		public override void GetSentients(ref List<ISentient> sentients)
		{
			if(official != null)
			{
				sentients.Add(official);
			}
		}

		public override void Initialize(Organization org)
		{
			AffiliatedOrganization = org;
		}

		public void SelectNewOfficial(Organization org, Faction affiliatedFaction, Race majorityRace, IOrganizationPosition top)
		{
			if(official == null || official.GetType() != typeof(Character))
			{
				var newLeader = new Character(35, gender, majorityRace, affiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10, true);
				newLeader.GenerateFamily(true, true);
				newLeader.OrganizationPosition = this;
				official = newLeader;
				EventManager.Instance.Dispatch(new AddContextEvent(newLeader, false));
				IncidentService.Instance.ReportStaticIncident("{TITLED:0} takes power.", new List<IIncidentContext>() { newLeader as IIncidentContext }, true);
				return;
			}

			var found = false;
			var previousOfficial = official as Character;
			Character newOfficial = null;
			if (previousOfficial != null && hereditary)
			{
				var viableChildren = previousOfficial.Children.Where(x => x.MajorCharacter && ContextDictionaryProvider.CurrentContexts[typeof(Character)].Contains(x)).ToList();
				if(gender != Gender.ANY)
				{
					viableChildren = viableChildren.Where(x => x.Gender == gender).ToList();
				}

				var viableChild = viableChildren.Count > 0 ? viableChildren.First() : null;
				if (viableChild != null)
				{
					newOfficial = viableChild;
					found = true;
				}
			}

			if (!found)
			{
				if (hereditary)
				{
					newOfficial = SimRandom.RandomRange(0, 5) < 4 && previousOfficial != null ? previousOfficial.CreateChild(true, gender) : new Character(35, gender, majorityRace, affiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10, true);
					newOfficial.GenerateFamily(true, true);
				}
				else
				{
					newOfficial = SimRandom.RandomRange(0, 5) < 1 && previousOfficial != null ? previousOfficial.CreateChild(true, gender) : new Character(35, gender, majorityRace, affiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10, true);
					newOfficial.GenerateFamily(true, true);
				}

				EventManager.Instance.Dispatch(new AddContextEvent(newOfficial, false));
			}

			newOfficial.OrganizationPosition = top == null ? this : top;
			official = newOfficial;
			var report = previousOfficial == null ? "{TITLED:0} takes power." : "{TITLED:0} succeeds {TITLED:1}.";
			IncidentService.Instance.ReportStaticIncident(report, new List<IIncidentContext>() { newOfficial, previousOfficial }, true);
		}

		public void HandleSuccession(IOrganizationPosition top = null)
		{
			SelectNewOfficial(AffiliatedOrganization, AffiliatedOrganization.AffiliatedFaction, SimRandom.RandomEntryFromList(AffiliatedOrganization.racesAllowedToHoldOffice), top);
		}

		public override bool TryFillNextPosition(out IOrganizationPosition filledPosition)
		{
			if (official == null)
			{
				HandleSuccession();
				filledPosition = this;
				return true;
			}
			else
			{
				filledPosition = null;
				return false;
			}
		}

		public void Update()
		{

		}

		public string GetTitle(ISentient sentient)
		{
			return titlePair.GetTitle(sentient.Gender);
		}
	}
}