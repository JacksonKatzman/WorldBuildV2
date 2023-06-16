using Game.Incidents;
using Game.Utilities;
using System;
using System.Collections.Generic;

namespace Game.Simulation
{
	public class AdventureCharacterContextCriteria : AdventureContextCriteria<Character>
	{
		public override Dictionary<string, Func<Character, string>> Replacements => replacements;
		private static Dictionary<string, Func<Character, string>> replacements = new Dictionary<string, Func<Character, string>>
		{
			{"{##}", (person) => string.Format("<i><link=\"{0}\">{1}</link></i>", person.ID, person.Name) },
			{"[##]", (person) => person.Gender == Enums.Gender.MALE ? "he" : "she" }
		};

		public override void RetrieveContext()
		{
			Context = SimRandom.RandomEntryFromList(ContextDictionaryProvider.CurrentContexts[typeof(Character)]);
		}

		public override void SpawnPopup()
		{

		}
	}
}
