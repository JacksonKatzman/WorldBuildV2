using Game.Enums;
using Game.Generators.Names;
using System;

namespace Game.Incidents
{
	public class MinorPerson : InertIncidentContext, IPerson
	{
		public override Type ContextType => typeof(MinorPerson);

		public override string Name => PersonName.GetTitledFullName(this);
		public CreatureName PersonName { get; set; }
		public int Age { get; set; }
		public Gender Gender { get; set; }
		public Race Race { get; set; }
		public Faction AffiliatedFaction { get; set; }
		public OrganizationPosition OfficialPosition { get; set; }

		public override void UpdateContext()
		{
			base.UpdateContext();
			if(this.CheckDestroyed())
			{
				Die();
			}
		}
	}
}