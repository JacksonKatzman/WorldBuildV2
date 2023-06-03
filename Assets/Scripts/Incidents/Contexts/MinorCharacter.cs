using Game.Enums;
using Game.Generators.Names;
using Game.Utilities;
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

		public MinorCharacter(int age, Gender gender, Faction affiliatedFaction)
		{
			Age = age;
			Gender = gender;
			AffiliatedFaction = affiliatedFaction;
		}

		public MinorCharacter(Faction affiliatedFaction)
		{
			AffiliatedFaction = affiliatedFaction;
			Race = affiliatedFaction.MajorityRace;
			Age = SimRandom.RandomRange(Race.MinAge, Race.MaxAge);
			Gender = (Gender)(SimRandom.RandomRange(0, 2));
			CharacterName = affiliatedFaction.namingTheme.GenerateName(Gender);
		}

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