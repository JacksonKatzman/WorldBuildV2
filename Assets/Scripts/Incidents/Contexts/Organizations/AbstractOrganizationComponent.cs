using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Game.Incidents
{
	[HideReferenceObjectPicker]
	public abstract class AbstractOrganizationComponent : IOrganizationAffiliated
	{
		public Organization AffiliatedOrganization { get; set; }
		public int OrganizationTier { get; set; }

		public abstract void Initialize(Organization org, int currentTier);
		public abstract bool Contains(ISentient sentient, out IOrganizationPosition pos);
		public abstract void GetPositionCount(ref int total, bool careAboutFilled);
		public abstract bool TryFillNextPosition(out IOrganizationPosition filledPosition);
		public abstract void GetSentients(ref List<ISentient> sentients);
	}
}