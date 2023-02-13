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
			{"{##}", (person) => person.Name }
		};

		//public GetOrCreatePersonAction GetOrCreatePerson;
		public override void RetrieveContext()
		{
			//temporary
			Context = SimRandom.RandomEntryFromList(SimulationManager.Instance.CurrentContexts[typeof(Person)]);
		}
	}
}
