using Game.Enums;
using Game.Generators.Names;
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
		 *		- It needs to be possible to have oligarchical systems
		 *		- Perhaps to make it easier for now they choose from predefined structures that weight likelihoods of tiers
		 *		- So it randomly chooses a 3/5/7 or something and just fills in those positions over time?'
		 *		- Would need buckets of responsibilites for them to grab from?
		 *		- Would RATHER do it with math tho
		 *		
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
			AddTier(new OrganizationPosition(AffiliatedFaction, majorityStartingRace, organizationType, 0, maxTiers));

			//point buy for titles

			//organization type? military/government/religious/etc

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
				var found = tier.Where(x => x.official == person).First();
				if(found != null)
				{
					position = found;
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

			var position = new OrganizationPosition();
			position.SelectNewOfficial(AffiliatedFaction, Leader.Race);
			var random = SimRandom.RandomRange(0, totalWeight + 1);
			for(int i = weights.Length - 1; i >= 0; i--)
			{
				random -= weights[i];
				if(random <= 0)
				{		
					hierarchy[i].Add(new OrganizationPosition(AffiliatedFaction, Leader.Race, organizationType, i, maxTiers));
					return;
				}
			}

			AddTier(new OrganizationPosition(AffiliatedFaction, Leader.Race, organizationType, hierarchy.Count, maxTiers));
		}

		private void AddTier(OrganizationPosition firstPosition)
		{
			var newTier = new OrganizationTier(firstPosition);
			hierarchy.Add(newTier);
		}
	}

	public class OrganizationPosition
	{
		public Person official;
		//points to spend on titles
		private int titlePoints;
		//title chosen
		public TitlePair titlePair;
		public OrganizationType organizationType;
		//responsibilities
		public OrganizationPosition() { }
		public OrganizationPosition(Faction affiliatedFaction, Race majorityRace, OrganizationType organizationType, int tier, int maxTiers)
		{
			this.organizationType = organizationType;
			titlePoints = maxTiers - tier;

			if(tier < 2)
			{
				SelectNewOfficial(affiliatedFaction, majorityRace);
			}

			titlePair = affiliatedFaction.namingTheme.GenerateTitle(organizationType, titlePoints);
		}
		public void SelectNewOfficial(Faction affiliatedFaction, Race majorityRace)
		{
			official = new Person(35, Enums.Gender.ANY, majorityRace, affiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10);
			SimulationManager.Instance.world?.AddContext(official);
		}
	}

	public class OrganizationTier : List<OrganizationPosition>
	{
		//whether or not all positions in this tier share titles/responsibilities such as high lords

		public OrganizationTier() { }
		public OrganizationTier(OrganizationPosition firstPostition)
		{
			Add(firstPostition);
		}
	}
}