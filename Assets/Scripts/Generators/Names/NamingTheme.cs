using Game.Data;
using Game.Enums;
using Game.Generators.Items;
using Game.Incidents;
using Game.Simulation;
using Game.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Generators.Names
{
	/*
	 * {A}: Adjective
	 * {N}: Noun
	 * {V}: Verb
	 * {T}: Town Noun
	 * {C}: Color
	 * {M}: Male First Name
	 * {F}: Female First Name
	 * {X}: Androgynous First Name
	 * {S}: Standard Surname (handle composites elsewhere)
	 * {Q}: Qualifier
	 */
	public class NamingTheme
	{
		public static string ADJECTIVE_MARKER = "{ADJ}";
		public static string NOUN_MARKER = "{NOUN}";
		public static string VERB_MARKER = "{VERB}";
		public static string PLACE_NOUN_MARKER = "{PLACENOUN}";
		public static string MALE_NAME_MARKER = "{MALE}";
		public static string FEMALE_NAME_MARKER = "{FEMALE}";
		public static string ANDROGYNOUS_NAME_MARKER = "{ANDRO}";
		public static string STANDARD_SURNAME_MARKER = "{SURNAME}";
		public static string QUALIFIER_MARKER = "{QUAL}";
		public static string FACTION_NAME_MARKER = "{FACTION}";
		public static string FACTION_PREFIX_MARKER = "{FACTIONPREFIX}";
		public static string FACTION_SUFFIX_MARKER = "{FACTIONSUFFIX}";

		public List<string> maleNameFormats;
		public List<string> femaleNameFormats;
		public List<string> surnameFormats;
		public List<string> townNameFormats;
		public List<string> factionNameFormats;

		public Dictionary<string, List<string>> nameData;

		public NamingTheme()
		{

		}

		/*
		 * for partial names, similar to markov chaining, we make dictionaries such that:
		 * the key for the fronts is the last letter
		 * the key for the backs is the first letter,
		 * and list of letter pairs that can be combined.
		 * 
		 * So then we just choose a letter pair, dictionary lookup a front and back based on that letter pair
		 * and combine them together
		 */

		public NamingTheme(NamingThemePreset preset)
		{
			maleNameFormats = new List<string>(preset.maleNameFormats);
			femaleNameFormats = new List<string>(preset.femaleNameFormats);
			surnameFormats = new List<string>(preset.compositeSurnameFormats);
			townNameFormats = new List<string>(preset.townNameFormats);
			factionNameFormats = new List<string>(preset.factionNameFormats);

			nameData = new Dictionary<string, List<string>>();
			nameData.Add(ADJECTIVE_MARKER, new List<string>());
			nameData.Add(NOUN_MARKER, new List<string>());
			nameData.Add(VERB_MARKER, new List<string>());
			nameData.Add(PLACE_NOUN_MARKER, new List<string>());
			nameData.Add(MALE_NAME_MARKER, new List<string>(preset.maleNames));
			nameData.Add(FEMALE_NAME_MARKER, new List<string>(preset.femaleNames));
			nameData.Add(ANDROGYNOUS_NAME_MARKER, new List<string>(preset.androgynousNames));
			nameData.Add(STANDARD_SURNAME_MARKER, new List<string>(preset.standardSurnames));
			nameData.Add(QUALIFIER_MARKER, new List<string>(preset.qualifiers));
			nameData.Add(FACTION_NAME_MARKER, new List<string>(preset.factionNames));
			nameData.Add(FACTION_PREFIX_MARKER, new List<string>(preset.factionPrefixes));
			nameData.Add(FACTION_SUFFIX_MARKER, new List<string>(preset.factionSuffixes));

			for (int i = 0; i < preset.themeCollections.Count; i++)
			{
				AddThemeCollection(preset.themeCollections[i]);
			}
		}

		public NamingTheme(NamingTheme other)
		{
			maleNameFormats = new List<string>(other.maleNameFormats);
			femaleNameFormats = new List<string>(other.femaleNameFormats);
			surnameFormats = new List<string>(other.surnameFormats);
			townNameFormats = new List<string>(other.townNameFormats);
			factionNameFormats = new List<string>(other.factionNameFormats);

			nameData = new Dictionary<string, List<string>>(other.nameData);
		}

		public string GenerateName(string format)
		{
			var textLine = string.Copy(format);
			var matches = Regex.Matches(textLine, @"\{(\w+)\}");

			foreach (Match match in matches)
			{
				var tag = match.Value;
				if (nameData.TryGetValue(tag, out var list))
				{
					var value = SimRandom.RandomEntryFromList(list);
					textLine = StringUtilities.ReplaceFirstOccurence(textLine, tag, value);
				}
			}

			return CapitalizeString(textLine);
		}

		//will probs put these in CharacterName
		public CharacterName GenerateSentientName(Gender gender, List<Character> parents = null)
		{
			string format = "";
			if(parents != null && parents[0].Gender == gender)
			{
				format = parents[0].CharacterName.nameFormat;
			}
			else
			{
				format = gender == Gender.MALE ? SimRandom.RandomEntryFromList(maleNameFormats) : SimRandom.RandomEntryFromList(femaleNameFormats);
			}

			var nameString = GenerateName(format);
			var split = nameString.Split(' ');
			var first = split[0];
			var last = split.Length > 1 ? split[split.Length - 1] : string.Empty;
			var middle = new List<string>();
			for(int i = 1; i < split.Length - 1; i++)
			{
				middle.Add(split[i]);
			}

			if(parents != null)
			{
				last = parents[0].CharacterName.surname;
			}

			return new CharacterName(format, first, last, middle);
		}

		public string GenerateFactionName()
		{
			//var currentFactionNames = ContextDictionaryProvider.GetAllContexts<Faction>().Select(x => x.Name);
			//var choices = nameData[FACTION_MARKER].Where(x => !currentFactionNames.Contains(x)).ToList();
			//return SimRandom.RandomEntryFromList(choices);
			var facNamesCount = (factionNameFormats.Count * nameData[FACTION_NAME_MARKER].Count);
			var combinationsCount = (nameData[FACTION_PREFIX_MARKER].Count * nameData[FACTION_SUFFIX_MARKER].Count);
			var total = facNamesCount + combinationsCount;
			var currentFactionNames = ContextDictionaryProvider.GetAllContexts<Faction>().Select(x => x.Name);
			if (SimRandom.RandomRange(0, total) <= facNamesCount)
			{
				var format = SimRandom.RandomEntryFromList(factionNameFormats);
				for(int i = 0; i < 10; i++)
				{
					var possibleName = GenerateName(format);
					if(!currentFactionNames.Contains(possibleName))
					{
						return possibleName;
					}
				}
				return GenerateName(format);
			}
			else
			{
				var possibleName = SimRandom.RandomEntryFromList(nameData[FACTION_PREFIX_MARKER]) + SimRandom.RandomEntryFromList(nameData[FACTION_SUFFIX_MARKER]);
				if(currentFactionNames.Contains(possibleName))
				{
					return GenerateFactionName();
				}
				else
				{
					return possibleName;
				}
			}
		}

		public string GenerateItemName(Item item)
		{
			return "ITEM";
		}

		public string GenerateItemName(Item item, Character creator)
		{
			return string.Format("{0}'s ITEM", creator.Name);
		}

		private void AddThemeCollection(NamingThemeCollection collection)
		{
			nameData[ADJECTIVE_MARKER].AddRange(collection.adjectives);
			nameData[NOUN_MARKER].AddRange(collection.nouns);
			nameData[VERB_MARKER].AddRange(collection.verbs);
			nameData[PLACE_NOUN_MARKER].AddRange(collection.placeNouns);
		}

		private string CapitalizeString(string toBeFormatted)
		{
			return Regex.Replace(toBeFormatted.ToLower(), @"((^\w)|(\s|\p{P})\w)", match => match.Value.ToUpper());
		}

		private string[] FormatTextAsset(TextAsset textAsset)
		{
			if (textAsset != null)
			{
				var longForm = textAsset.text;
				char[] separators = new char[] { '\n' };
				var split = longForm.Split(separators, StringSplitOptions.RemoveEmptyEntries);
				return split;
			}
			else
			{
				return new string[0];
			}
		}
	}
}
