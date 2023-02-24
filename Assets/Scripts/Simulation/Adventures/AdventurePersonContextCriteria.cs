using Game.Incidents;
using Game.Utilities;
using System;
using System.Collections.Generic;

namespace Game.Simulation
{
	public class AdventurePersonContextCriteria : AdventureContextCriteria<Person>
	{
		public override Dictionary<string, Func<Person, string>> Replacements => replacements;
		private static Dictionary<string, Func<Person, string>> replacements = new Dictionary<string, Func<Person, string>>
		{
			{"{##}", (person) => string.Format("<i><link=\"{0}\">{1}</link></i>", person.ID, person.Name) },
			{"[##]", (person) => person.Gender == Enums.Gender.MALE ? "he" : "she" }
		};

		public override void RetrieveContext()
		{
			Context = SimRandom.RandomEntryFromList(SimulationManager.Instance.CurrentContexts[typeof(Person)]);
		}
	}
}
