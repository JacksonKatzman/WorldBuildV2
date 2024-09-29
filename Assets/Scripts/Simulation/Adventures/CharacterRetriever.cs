using Game.Enums;
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

		private static readonly Dictionary<string, Func<Character, int, string>> replacements = new Dictionary<string, Func<Character, int, string>>
		{
			{"FIRST", (person, criteriaID) => person.Link(person.CharacterName.firstName) },
			{"SUBJ", (person, criteriaID) => person.Link(person.Gender.Subject()) },
			{"OBJ", (person, criteriaID) => person.Link(person.Gender.Object()) },
			{"DPOS", (person, criteriaID) => person.Link(person.Gender.DependantPossessive()) },
			{"IPOS", (person, criteriaID) => person.Link(person.Gender.IndependantPossessive()) },
			{"SUBCONT", (person, criteriaID) => person.Link(person.Gender.SubjectContraction()) }
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

		override public void ReplaceTextPlaceholders(ref string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}

			HandleTextReplacements(ref text, replacements);
			base.ReplaceTextPlaceholders(ref text);
		}

		public override void SpawnPopup()
		{

		}
	}
}
