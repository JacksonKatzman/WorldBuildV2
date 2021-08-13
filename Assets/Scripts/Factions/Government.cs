using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Data.EventHandling;

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

		public bool IsLeader(Person person, int atOrAboveLevel)
		{
			atOrAboveLevel = Mathf.Min(atOrAboveLevel, leadershipStructure.Count);

			for(int i = 0; i <= atOrAboveLevel; i++)
			{
				foreach(LeadershipStructureNode node in leadershipStructure[i])
				{
					if(node.occupant == person)
					{
						return true;
					}
				}
			}

			return false;
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
			foreach (LeadershipTier tier in leadershipStructure)
			{
				foreach (LeadershipStructureNode node in tier)
				{
					if(node.occupant == simEvent.person)
					{
						DetermineNewLeader(node);
						break;
					}
				}
			}
		}

		private void DetermineNewLeader(LeadershipStructureNode node)
		{
			node.occupant = PersonGenerator.GeneratePerson(faction, node.ageRange, node.requiredGender, 100, node);
		}

		private void SubscribeToEvents()
		{
			EventManager.Instance.AddEventHandler<PersonDiedEvent>(OnPersonDeath);
		}
	}
}