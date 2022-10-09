using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Data.EventHandling;
using Game.Enums;
using Game.Holiday;

namespace Game.Factions
{
	public class Government
	{
		public List<LeadershipTier> leadershipStructure;
		public List<Holiday.Holiday> holidays;
		private OldFaction faction;
		public Priorities priorities;
		public FactionStats stats;

		private int numberOfTimesUpgraded = 0;
		private int turnsSinceLastUpgrade = 0;

		public Government(OldFaction faction)
		{
			this.faction = faction;
			priorities = new Priorities(0, 0, 0, 0, 0);
			stats = new FactionStats();

			holidays = new List<Holiday.Holiday>();

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

		public void UpdateFactionUsingPassiveTraits(OldFaction faction)
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

		public void BuildNewGovernmentTier()
		{
			var initialTierCount = leadershipStructure.Count;
			var positionsInNewTier = SimRandom.RandomRange(initialTierCount + 1, (initialTierCount + 1) * 2 + 1);
			var tier = new LeadershipTier();
			var ageRange = new Vector2Int(SimRandom.RandomRange(16, 56), SimRandom.RandomRange(56, 76));

			for (int i = 0; i < positionsInNewTier; i++)
			{
				tier.tier.Add(new LeadershipStructureNode(ageRange, (Gender)SimRandom.RandomRange(0, 3)));
			}

			leadershipStructure.Add(tier);

			NameGenerator.GenerateTitleStructure(leadershipStructure, priorities, faction.race);

			if (initialTierCount <= 3)
			{
				foreach (LeadershipStructureNode node in tier)
				{
					DetermineNewLeader(node);
				}
			}
		}

		public void AddLeaderToRandomTierWeighted()
		{
			var weighted = new Dictionary<int, List<LeadershipTier>>();
			for(int i = 0; i < leadershipStructure.Count; i++)
			{
				weighted.Add((int)Mathf.Pow((i+1), 2), new List<LeadershipTier>{ leadershipStructure[i] });
			}

			var chosenTier = SimRandom.RandomEntryFromWeightedDictionary(weighted);
			var ageRange = new Vector2Int(SimRandom.RandomRange(16, 56), SimRandom.RandomRange(56, 76));
			chosenTier.tier.Add(new LeadershipStructureNode(ageRange, (Gender)SimRandom.RandomRange(0, 3)));
		}

		private void BuildInitialLeadershipStructure()
		{
			leadershipStructure = new List<LeadershipTier>();

			var leadershipTier = new LeadershipTier();
			leadershipTier.tier.Add(new LeadershipStructureNode(new Vector2Int(18, 68), Gender.ANY));

			leadershipStructure.Add(leadershipTier);

			NameGenerator.GenerateTitleStructure(leadershipStructure, priorities, faction.race);

			foreach(LeadershipTier tier in leadershipStructure)
			{
				foreach(LeadershipStructureNode node in tier)
				{
					DetermineNewLeader(node);
				}
			}
		}

		private void OnPersonDeath(CreatureDiedEvent simEvent)
		{
			if (simEvent.creature is Person person)
			{
				if (person.governmentOffice != null && person.faction.government == this)
				{
					DetermineNewLeader(person.governmentOffice);
				}
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
			EventManager.Instance.AddEventHandler<CreatureDiedEvent>(OnPersonDeath);
		}
	}
}