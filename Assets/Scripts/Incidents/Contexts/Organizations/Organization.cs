using Game.Enums;
using Game.Simulation;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public class Organization : IFactionAffiliated
	{
		public Faction AffiliatedFaction { get; set; }
		public Person Leader => hierarchy.First().First().official;
		/*Political structure with included title structure
		 * - Levels of government and titles of those levels should be mutable and addable via incidents
		 * - A way of adding flavor as to the responsibilites of each level?
		 * - Only a very few of the levels should be populated with great people in the sim
		 *		- Things like head of state/church/treasury etc
		 *		- Perhaps a flavor list of responsibilities that gets generated at random
		 *	- Need a way of determining at what level to add a new position
		 *		Weight at n (where n = index of current tier) = ceil((Total Height of subtree + total count of nodes in subtree + 1) * (n+1)^2) - ceil(nodes in current tier * height of subtree^2)
		 */

		public List<OrganizationTier> hierarchy;
		public OrganizationType organizationType;
		private int maxTiers = 7;

		public Organization() { }
		public Organization(Faction faction, Race majorityStartingRace, OrganizationType organizationType)
		{
			AffiliatedFaction = faction;
			this.organizationType = organizationType;
			EventManager.Instance.AddEventHandler<RemoveContextEvent>(OnRemoveContextEvent);

			hierarchy = new List<OrganizationTier>();
			AddTier(majorityStartingRace);
		}

		public void OnRemoveContextEvent(RemoveContextEvent gameEvent)
		{
			if(gameEvent.context.GetType() == typeof(Person) && Contains((Person)gameEvent.context, out var position))
			{
				position.SelectNewOfficial(AffiliatedFaction, Leader.Race);
			}
		}

		public bool Contains(Person person, out OrganizationPosition position)
		{
			foreach(var tier in hierarchy)
			{
				var found = tier.Where(x => x.official == person);
				if(found.Count() > 0)
				{
					position = found.First();
					return true;
				}
			}

			position = null;
			return false;
		}

		public void UpdateHierarchy()
		{
			int[] weights = new int[hierarchy.Count];
			var totalWeight = 0;
			for(int i = 0; i < weights.Length; i++)
			{
				var totalHeight = weights.Length - i;
				var totalCount = 0;
				for(int j = i; j < weights.Length; j++)
				{
					totalCount += hierarchy[j].Count;
				}

				var weight = (int)(((totalHeight + totalCount + 1) * Mathf.Pow(i + 1, 2)) - (hierarchy[i].Count * Mathf.Pow(totalHeight, 2)));
				weights[i] = weight;
				totalWeight += weight;
			}

			//to account for an empty possible tier
			if (hierarchy.Count < maxTiers)
			{
				totalWeight += (int)(2 * Mathf.Pow(weights.Length + 1, 2));
			}

			var random = SimRandom.RandomRange(0, totalWeight + 1);
			for(int i = weights.Length - 1; i >= 0; i--)
			{
				random -= weights[i];
				if(random <= 0)
				{
					hierarchy[i].AddPosition(AffiliatedFaction, Leader.Race);
					return;
				}
			}

			AddTier(Leader.Race);
		}

		private void AddTier(Race race)
		{
			var newTier = new OrganizationTier(AffiliatedFaction, race, organizationType, hierarchy.Count, maxTiers);
			hierarchy.Add(newTier);
		}
	}
}