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
		private Faction faction;
		public Priorities priorities;

		private int numberOfTimesUpgraded = 0;
		private int turnsSinceLastUpgrade = 0;

		public Government(Faction faction)
		{
			this.faction = faction;
			priorities = new Priorities(0, 0, 0, 0, 0);

			BuildInitialLeadershipStructure();

			SubscribeToEvents();
		}

		public void HandleUpgrades(int population)
		{
			if(turnsSinceLastUpgrade >= 10 && population > (100 * Mathf.Pow(2, numberOfTimesUpgraded)))
			{
				SimAIManager.Instance.CallGovernmentUpgrade(this);
				turnsSinceLastUpgrade = 0;
				numberOfTimesUpgraded++;
			}
			else
			{
				turnsSinceLastUpgrade++;
			}
		}

		public void UpdateFactionUsingPassiveTraits(Faction faction)
		{
			/*
			foreach(GovernmentTrait trait in governmentType.traits)
			{
				if(trait is PassiveGovernmentTrait)
				{
					trait.Invoke(faction);
				}
			}
			*/
		}

		private void BuildInitialLeadershipStructure()
		{
			leadershipStructure = new List<LeadershipTier>();

			var leadershipTier = new LeadershipTier();
			leadershipTier.tier.Add(new LeadershipStructureNode(new Vector2Int(18, 68), Gender.ANY));

			leadershipStructure.Add(leadershipTier);

			NameGenerator.GenerateTitleStructure(leadershipStructure, priorities);

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
			person.priorities += priorities;
			PersonGenerator.RegisterPerson(person);
		}

		private void SubscribeToEvents()
		{
			EventManager.Instance.AddEventHandler<PersonDiedEvent>(OnPersonDeath);
		}
	}
}