using Game.Simulation;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class SubOrganization : AbstractOrganizationComponent
	{
		public string subOrganizationTitle;
		public List<OrganizationTier> tiers = new List<OrganizationTier>();

		public SubOrganization() { }

		public SubOrganization(SubOrganization other)
		{
			subOrganizationTitle = other.subOrganizationTitle;
			tiers = new List<OrganizationTier>();
			foreach(var tier in other.tiers)
			{
				tiers.Add(new OrganizationTier(tier));
			}
		}

		override public void Initialize(Organization org, int currentTier)
		{
			AffiliatedOrganization = org;
			OrganizationTier = currentTier;

			for(int i = 0; i < tiers.Count; i++)
			{
				tiers[i].Initialize(org, currentTier + i);
			}
		}

		public override bool Contains(ISentient sentient, out IOrganizationPosition position)
		{
			foreach(var tier in tiers)
			{
				if(tier.Contains(sentient, out position))
				{
					return true;
				}
			}

			position = null;
			return false;
		}

		public override bool TryFillNextPosition(out IOrganizationPosition filledPosition)
		{
			var checkedTiers = 0;
			var filled = false;
			while(!filled && checkedTiers < tiers.Count)
			{
				var tier = tiers[checkedTiers];
				filled = tier.TryFillNextPosition(out filledPosition);
				if (filled)
				{
					return true;
				}
				checkedTiers++;
			}

			filledPosition = null;
			return false;
		}

		public override void GetPositionCount(ref int total, bool careAboutFilled)
		{
			foreach(var tier in tiers)
			{
				tier.GetPositionCount(ref total, careAboutFilled);
			}
		}

		public override void GetSentients(ref List<ISentient> sentients)
		{
			foreach (var tier in tiers)
			{
				tier.GetSentients(ref sentients);
			}
		}
	}
}