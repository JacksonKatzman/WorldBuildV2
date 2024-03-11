using Game.Incidents;
using Game.Utilities;
using System;
using System.Collections.Generic;

namespace Game.Simulation
{
	public class CharacterCriteria : AdventureContextCriteria<Character>
	{
		public ContextualIncidentActionField<Character> character;
		public override Dictionary<string, Func<Character, int, string>> Replacements => replacements;
		private static readonly Dictionary<string, Func<Character, int, string>> replacements = new Dictionary<string, Func<Character, int, string>>
		{
			{"{##}", (person, criteriaID) => string.Format("<i><link=\"{0}\">{1}</link></i>", criteriaID, person.Name) },
			{"[##]", (person, criteriaID) => person.Gender == Enums.Gender.MALE ? "he" : "she" }
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
