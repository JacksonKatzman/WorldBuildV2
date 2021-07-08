using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Factions
{
	public class Government
	{
		public List<List<Person>> leadershipStructure;
		public GovernmentType governmentType;
		private Faction faction;

		public Government(Faction faction, GovernmentType type)
		{
			this.faction = faction;
			governmentType = type;

			BuildLeadershipStructure();
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
			leadershipStructure = new List<List<Person>>();
			for(int tierIndex = 0; tierIndex < governmentType.leadershipStructure.Count; tierIndex++)
			{
				var tier = new List<Person>();
				foreach(LeadershipStructureNode node in governmentType.leadershipStructure[tierIndex])
				{
					tier.Add(PersonGenerator.GeneratePerson(faction, node.ageRange, node.requiredGender));
				}
			}
		}
	}
}