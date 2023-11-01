using Game.Data;
using Game.Enums;
using Game.Utilities;
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
			var parents = new List<Character>();
			var p1 = parent.GetTypedFieldValue();
			if(p1 != null)
			{
				parents.Add(p1);
				if(p1.Spouses.Count > 0)
				{
					parents.Add(SimRandom.RandomEntryFromList(p1.Spouses));
				}
			}

			var newPerson = new Character(age, gender, race.GetTypedFieldValue().AffiliatedRace, faction.GetTypedFieldValue().AffiliatedFaction, politicalPriority,
				economicPriority, religiousPriority, militaryPriority, influence, wealth, strength, dexterity, constitution,
				intelligence, wisdom, charisma, majorCharacter, parents);

			if(generateFamily)
			{
				var shouldGenerateParents = parents.Count == 0 ? true : false;
				newPerson.GenerateFamily(shouldGenerateParents, 0.0f, 0);
			}

			return newPerson;
		}

		override protected void Complete()
		{
			if (madeNew)
			{
				var character = actionField.GetTypedFieldValue();
				foreach (var parent in character.Parents)
				{
					parent.Children.Add(character);
					if (parent.HasOrganizationPosition)
					{
						parent.OrganizationPosition.Update();
					}
				}
			}
			base.Complete();
		}

		protected override void OnAllowCreateValueChanged()
		{
			race.enabled = allowCreate;
			faction.enabled = allowCreate;
		}

		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			return faction.actionField.CalculateField(context) && race.actionField.CalculateField(context) && parent.CalculateField(context);
		}
	}
}