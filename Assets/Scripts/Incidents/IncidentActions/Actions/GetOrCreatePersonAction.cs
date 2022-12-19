using Game.Enums;
using Game.Simulation;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class GetOrCreatePersonAction : GetOrCreateAction<Person>
	{
		[ShowIf("@this.allowCreate")]
		public ContextualIncidentActionField<Person> parent;
		[ShowIf("@this.allowCreate")]
		public ContextualIncidentActionField<Race> race;
		[ShowIf("@this.allowCreate")]
		public ContextualIncidentActionField<Faction> faction;
		[ShowIf("@this.allowCreate")]
		public Gender gender;
		[ShowIf("@this.allowCreate")]
		public IntegerRange age;
		[ShowIf("@this.allowCreate")]
		public IntegerRange politicalPriority;
		[ShowIf("@this.allowCreate")]
		public IntegerRange economicPriority;
		[ShowIf("@this.allowCreate")]
		public IntegerRange religiousPriority;
		[ShowIf("@this.allowCreate")]
		public IntegerRange militaryPriority;
		[ShowIf("@this.allowCreate")]
		public IntegerRange influence;
		[ShowIf("@this.allowCreate")]
		public IntegerRange wealth;
		[ShowIf("@this.allowCreate")]
		public IntegerRange strength;
		[ShowIf("@this.allowCreate")]
		public IntegerRange dexterity;
		[ShowIf("@this.allowCreate")]
		public IntegerRange constitution;
		[ShowIf("@this.allowCreate")]
		public IntegerRange intelligence;
		[ShowIf("@this.allowCreate")]
		public IntegerRange wisdom;
		[ShowIf("@this.allowCreate")]
		public IntegerRange charisma;

		protected override Person MakeNew()
		{
			var parents = parent.GetTypedFieldValue() != null ? new List<Person>() { parent.GetTypedFieldValue() } : null;
			var newPerson = new Person(age, gender, race.GetTypedFieldValue(), faction.GetTypedFieldValue(), politicalPriority,
				economicPriority, religiousPriority, militaryPriority, influence, wealth, strength, dexterity, constitution,
				intelligence, wisdom, charisma, null, parents);

			return newPerson;
		}

		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			return faction.CalculateField(context) && race.CalculateField(context);
		}
	}
}