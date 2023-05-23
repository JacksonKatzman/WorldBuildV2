using Game.Enums;
using Game.Generators.Names;
using System;

namespace Game.Incidents
{
	public class MinorCharacter : InertIncidentContext, ICharacter, IFactionAffiliated
	{
		public override Type ContextType => typeof(MinorCharacter);

		public override string Name => CharacterName.GetTitledFullName(this);
		public CharacterName CharacterName { get; set; }
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