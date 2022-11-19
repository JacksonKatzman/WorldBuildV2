using Game.Enums;
using Game.Simulation;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class GetOrCreatePersonAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Person> person;

		public ContextualIncidentActionField<Person> parent;
		public ContextualIncidentActionField<Race> race;
		public ContextualIncidentActionField<Faction> faction;
		public Gender gender;
		public IntegerRange age;
		public IntegerRange politicalPriority;
		public IntegerRange economicPriority;
		public IntegerRange religiousPriority;
		public IntegerRange militaryPriority;
		public IntegerRange influence;
		public IntegerRange wealth;
		public IntegerRange strength;
		public IntegerRange dexterity;
		public IntegerRange constitution;
		public IntegerRange intelligence;
		public IntegerRange wisdom;
		public IntegerRange charisma;
		public ActionResultField<Person> personResult;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			if (person.GetTypedFieldValue() == null)
			{
				var parents = parent.GetTypedFieldValue() != null ? new List<Person>() { parent.GetTypedFieldValue() } : null;
				var newPerson = new Person(age, gender, race.GetTypedFieldValue(), faction.GetTypedFieldValue(), politicalPriority,
					economicPriority, religiousPriority, militaryPriority, influence, wealth, strength, dexterity, constitution,
					intelligence, wisdom, charisma, null, parents);
				SimulationManager.Instance.world.AddContext(newPerson);
				OutputLogger.Log("Person Created!");
			}
			else
			{
				personResult.SetValue(person.GetTypedFieldValue());
				OutputLogger.Log("Person Found!");
			}
		}
	}
}