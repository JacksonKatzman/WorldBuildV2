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
		private Faction faction;

		public Government(Faction faction, GovernmentType type)
		{
			this.faction = faction;
			governmentType = type;

			BuildLeadershipStructure();

			SubscribeToEvents();
		}

		public void UpdateFactionUsingPassiveTraits(Faction faction)
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
			foreach (LeadershipTier tier in leadershipStructure)
			{
				foreach (LeadershipStructureNode node in tier)
				{
					if(node.occupant == simEvent.person)
					{
						DetermineNewLeader(node);
					}
				}
			}
		}

		private void DetermineNewLeader(LeadershipStructureNode node)
		{
			node.occupant = PersonGenerator.GeneratePerson(faction, node.ageRange, node.requiredGender);
		}

		private void SubscribeToEvents()
		{
			EventManager.Instance.AddEventHandler<PersonDiedEvent>(OnPersonDeath);
		}
	}
}