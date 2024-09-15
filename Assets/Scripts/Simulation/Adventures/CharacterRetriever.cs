using Game.Incidents;
using Game.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Simulation
{
	public class CharacterRetriever : AdventureContextRetriever<Character>
	{
		public bool preferKnownCharacter;

		public CharacterTrait preferedTrait;
		public override Dictionary<string, Func<Character, int, string>> Replacements => replacements;
		private static readonly Dictionary<string, Func<Character, int, string>> replacements = new Dictionary<string, Func<Character, int, string>>
		{
			{"{##}", (person, criteriaID) => string.Format("<i><link=\"{0}\">{1}</link></i>", criteriaID, person.CharacterName.FirstName) },
			{"[##]", (person, criteriaID) => person.Gender == Enums.Gender.MALE ? "he" : "she" }
		};

		public override Character RetrieveContext()
		{
			//if theres someone we know NEARBY who FITS THE CRITERIA
			//else if we are allowed to use major characters see if one is nearby and meets criteria
			//if all else fails, make a new one
			List<Character> possibilities = new List<Character>();
			if(preferKnownCharacter)
            {
				possibilities = KnownContexts == null ? null : KnownContexts.Where(x => x.CharacterTraits.Contains(preferedTrait)).ToList();
            }
			if(possibilities == null || possibilities.Count == 0)
            {
				possibilities = World.CurrentWorld.People.Where(x => x.CharacterTraits.Contains(preferedTrait) && x.CurrentLocation.GetDistanceBetweenLocations(AdventureService.Instance.CurrentLocation) < 10).ToList();
			}

			if (possibilities.Count > 0)
			{
				return SimRandom.RandomEntryFromList(possibilities);
			}
			else
			{
				var character = new Character(SimRandom.RandomEntryFromList(World.CurrentWorld.Factions));
				return character;
			}
		}

		public override void SpawnPopup()
		{

		}
	}
}
