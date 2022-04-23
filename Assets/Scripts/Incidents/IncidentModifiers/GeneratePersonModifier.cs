using Game.Enums;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class GeneratePersonModifier : IncidentModifier
	{
		private List<RoleType> roles;

		public GeneratePersonModifier(List<IIncidentTag> tags, float probability, List<RoleType> roles) : base(tags, probability)
		{
			this.roles = roles;
		}

		public override void Setup()
		{
			base.Setup();
			var person = new Person(roles);
			PersonGenerator.RegisterPerson(person);
			ProvideModifierInfo(x => (x as ICreatureContainer)?.Creatures.Add(person));
		}
	}
}