using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Data.EventHandling;
using Game.Enums;

namespace Game.Factions
{
	public class Government
	{
		public List<LeadershipTier> leadershipStructure;
		public GovernmentType governmentType;
		private FactionSimulator faction;

		public Government(FactionSimulator faction, GovernmentType type)
		{
			this.faction = faction;
			governmentType = type;

			BuildLeadershipStructure();

			SubscribeToEvents();
		}

		public Government(FactionSimulator faction) : this (faction, new GovernmentType(faction))
		{

		}

		public void UpdateFactionUsingPassiveTraits(FactionSimulator faction)
		{
			foreach(GovernmentTrait trait in governmentType.traits)
			{
				if(trait is PassiveGovernmentTrait)
				{
					trait.Invoke(faction);
				}
			}
		}

		private void BuildLeadershipStructure()
		{
			leadershipStructure = new List<LeadershipTier>();

			foreach(LeadershipTier tier in governmentType.leadershipStructure)
			{
				leadershipStructure.Add(new LeadershipTier(tier));
			}

			foreach(LeadershipTier tier in leadershipStructure)
			{
				foreach(LeadershipStructureNode node in tier)
				{
					DetermineNewLeader(node);
				}
			}
		}

		private void OnPersonDeath(PersonDiedEvent simEvent)
		{
			if (simEvent.person.governmentOffice != null && simEvent.person.faction.government == this)
			{
				DetermineNewLeader(simEvent.person.governmentOffice);
			}
		}

		private void DetermineNewLeader(LeadershipStructureNode node)
		{
			var person = new Person(faction, SimRandom.RandomRange(node.ageRange.x, node.ageRange.y), node.requiredGender, 100, new List<RoleType>(), node);
			PersonGenerator.RegisterPerson(person);
		}

		private void SubscribeToEvents()
		{
			EventManager.Instance.AddEventHandler<PersonDiedEvent>(OnPersonDeath);
		}
	}
}