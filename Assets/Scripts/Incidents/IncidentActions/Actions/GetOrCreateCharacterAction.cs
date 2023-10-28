using Game.Data;
using Game.Enums;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class GetOrCreateCharacterAction : GetOrCreateAction<Character>
	{
		[ShowIf("@this.allowCreate")]
		public ContextualIncidentActionField<Character> parent;
		[ShowIf("@this.allowCreate")]
		//public ContextualIncidentActionField<Race> race;
		public InterfacedIncidentActionFieldContainer<IRaceAffiliated> race;
		[ShowIf("@this.allowCreate")]
		//public ContextualIncidentActionField<Faction> faction;
		public InterfacedIncidentActionFieldContainer<IFactionAffiliated> faction;
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
		[ShowIf("@this.allowCreate")]
		public bool majorCharacter = true;
		[ShowIf("@this.allowCreate")]
		public bool generateFamily = true;

		protected override Character MakeNew()
		{
			var parents = parent.GetTypedFieldValue() != null ? new List<Character>() { parent.GetTypedFieldValue() } : null;
			var newPerson = new Character(age, gender, race.GetTypedFieldValue().AffiliatedRace, faction.GetTypedFieldValue().AffiliatedFaction, politicalPriority,
				economicPriority, religiousPriority, militaryPriority, influence, wealth, strength, dexterity, constitution,
				intelligence, wisdom, charisma, majorCharacter, parents);

			if(generateFamily)
			{
				newPerson.GenerateFamily(true, true);
			}

			foreach(var parent in newPerson.Parents)
			{
				parent.Children.Add(newPerson);
			}

			return newPerson;
		}

		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			return faction.actionField.CalculateField(context) && race.actionField.CalculateField(context) && parent.CalculateField(context);
		}
	}
}