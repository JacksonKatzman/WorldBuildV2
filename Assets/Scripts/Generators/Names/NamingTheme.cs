using Game.Data;
using Game.Enums;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Game.Generators.Names
{
	public class NamingTheme
	{
		public ModifiableWeightedCollection nouns;
		public ModifiableWeightedCollection verbs;
		public ModifiableWeightedCollection adjectives;
		public ModifiableWeightedCollection townNouns;

		public ModifiableWeightedCollection consonants;
		public ModifiableWeightedCollection beginningConsonants;
		public ModifiableWeightedCollection endConsonants;

		public ModifiableWeightedCollection vowels;
		public ModifiableWeightedCollection beginningVowels;
		public ModifiableWeightedCollection endVowels;

		public List<string> maleNames;
		public List<string> femaleNames;

		public Dictionary<OrganizationType, TitleDictionary> titles;
		public List<string> titleQualifiers;

		public int minFirstNameSyllables;
		public int maxFirstNameSyllables;
		public int minSurnameSyllables;
		public int maxSurnameSyllables;
		public Dictionary<int, List<string>> personNameFormats;
		public Dictionary<int, List<string>> townNameFormats;
		public Dictionary<int, List<string>> factionNameFormats;

		private string currentNameFormat;

		public NamingTheme(NamingThemePreset preset)
		{
			nouns = new ModifiableWeightedCollection();
			verbs = new ModifiableWeightedCollection();
			adjectives = new ModifiableWeightedCollection();
			townNouns = new ModifiableWeightedCollection();

			consonants = new ModifiableWeightedCollection();
			beginningConsonants = new ModifiableWeightedCollection();
			endConsonants = new ModifiableWeightedCollection();

			vowels = new ModifiableWeightedCollection();
			beginningVowels = new ModifiableWeightedCollection();
			endVowels = new ModifiableWeightedCollection();

			maleNames = new List<string>();
			femaleNames = new List<string>();

			titles = new Dictionary<OrganizationType, TitleDictionary>();
			titleQualifiers = new List<string>();

			for(int i = 0; i < preset.themeCollections.Count; i++)
			{
				AddThemeCollection(preset.themeCollections[i]);
			}

			minFirstNameSyllables = preset.minFirstNameSyllables;
			maxFirstNameSyllables = preset.maxFirstNameSyllables;
			minSurnameSyllables = preset.minSurnameSyllables;
			maxSurnameSyllables = preset.maxSurnameSyllables;
			personNameFormats = preset.personNameFormats;
			townNameFormats = preset.townNameFormats;
			factionNameFormats = preset.factionNameFormats;

			SetupNameFormat();
		}

		public string GenerateName<Person>(Gender gender)
		{
			var format = string.Copy(currentNameFormat);

			return FillOutFormat(format, gender);
		}

		public string GenerateTownName()
		{
			var format = SimRandom.RandomEntryFromWeightedDictionary(townNameFormats);

			return FillOutFormat(format, Gender.ANY);
		}

		public string GenerateFactionName()
		{
			var format = SimRandom.RandomEntryFromWeightedDictionary(factionNameFormats);

			return FillOutFormat(format, Gender.ANY);
		}

		public string GenerateTerrainName(string format)
		{
			return FillOutFormat(format, Gender.ANY);
		}

		public TitlePair GenerateTitle(OrganizationType titleType, int points)
		{
			var useQualifier = SimRandom.RandomTrueFalse();
			if (useQualifier)
			{
				var list = titles[titleType][points-1];
				var titlePair = new TitlePair(SimRandom.RandomEntryFromList(list.titlePairs));
				var qualifier = SimRandom.RandomEntryFromList(titleQualifiers);
				//need to make it so that the male and females get the same format fill
				titlePair.maleTitle = CapitalizeString(string.Format(qualifier, FillOutFormat(titlePair.maleTitle, Gender.MALE)));
				titlePair.femaleTitle = CapitalizeString(string.Format(qualifier, FillOutFormat(titlePair.femaleTitle, Gender.FEMALE)));
				return titlePair;
			}
			else
			{
				var list = titles[titleType][points];
				var titlePair = new TitlePair(SimRandom.RandomEntryFromList(list.titlePairs));
				//need to make it so that the male and females get the same format fill
				titlePair.maleTitle = FillOutFormat(titlePair.maleTitle, Gender.MALE);
				titlePair.femaleTitle = FillOutFormat(titlePair.femaleTitle, Gender.FEMALE);
				return titlePair;
			}
		}

		private void AddThemeCollection(NamingThemeCollection collection)
		{
			nouns.AddWeightedStrings(collection.nouns);
			verbs.AddWeightedStrings(collection.verbs);
			adjectives.AddWeightedStrings(collection.adjectives);
			townNouns.AddWeightedStrings(collection.townNouns);

			consonants.AddWeightedStrings(collection.consonants);
			beginningConsonants.AddWeightedStrings(collection.consonants.Where(x => x.allowedAtBeginning == true).ToList());
			endConsonants.AddWeightedStrings(collection.consonants.Where(x => x.allowedAtEnd == true).ToList());

			vowels.AddWeightedStrings(collection.vowels);
			beginningVowels.AddWeightedStrings(collection.vowels.Where(x => x.allowedAtBeginning == true).ToList());
			endVowels.AddWeightedStrings(collection.vowels.Where(x => x.allowedAtEnd == true).ToList());

			foreach(var asset in collection.maleNames)
			{
				char[] delims = new[] { '\n' };
				var names = asset.text.Split(delims, StringSplitOptions.RemoveEmptyEntries);
				maleNames.AddRange(names);
			}

			foreach (var asset in collection.femaleNames)
			{
				char[] delims = new[] { '\n' };
				var names = asset.text.Split(delims, StringSplitOptions.RemoveEmptyEntries);
				femaleNames.AddRange(names);
			}

			foreach(var upperPair in collection.titles)
			{
				if(!titles.ContainsKey(upperPair.Key))
				{
					titles.Add(upperPair.Key, upperPair.Value);
				}
				else
				{
					titles[upperPair.Key].Merge(collection.titles[upperPair.Key]);
				}
			}

			titleQualifiers = titleQualifiers.Union(collection.titleQualifiers).ToList();
		}

		private string CapitalizeString(string toBeFormatted)
		{
			return Regex.Replace(toBeFormatted.ToLower(), @"((^\w)|(\s|\p{P})\w)", match => match.Value.ToUpper());
		}

		private void SetupNameFormat()
		{
			currentNameFormat = SimRandom.RandomEntryFromWeightedDictionary(personNameFormats);
			while(currentNameFormat.Contains("{P}"))
			{
				currentNameFormat = ReplaceFirstOccurence(currentNameFormat, "{P}", GenerateSyllabicName(2, 5));
			}
		}

		private string ReplaceFirstOccurence(string source, string find, string replace)
		{
			int place = source.IndexOf(find);
			string result = source.Remove(place, find.Length).Insert(place, replace);
			return result.Replace("\r", "");
		}

		private string FillOutFormat(string format, Gender gender)
		{
			var result = format;

			while(result.Contains("{F}"))
			{
				result = ReplaceFirstOccurence(result,"{F}", GenerateFirstName(gender));
			}
			while(result.Contains("{S}"))
			{
				result = ReplaceFirstOccurence(result,"{S}", GenerateSurname());
			}
			while(result.Contains("{A}"))
			{
				result = ReplaceFirstOccurence(result,"{A}", SimRandom.RandomEntryFromWeightedDictionary(adjectives.dictionary));
			}
			while (result.Contains("{T}"))
			{
				result = ReplaceFirstOccurence(result, "{T}", SimRandom.RandomEntryFromWeightedDictionary(townNouns.dictionary));
			}
			while (result.Contains("{V}"))
			{
				result = ReplaceFirstOccurence(result, "{V}", SimRandom.RandomEntryFromWeightedDictionary(verbs.dictionary));
			}
			while(result.Contains("{N}"))
			{
				result = ReplaceFirstOccurence(result, "{N}", SimRandom.RandomEntryFromWeightedDictionary(nouns.dictionary));
			}
			while (result.Contains("{Q}"))
			{
				result = ReplaceFirstOccurence(result, "{Q}", SimRandom.RandomEntryFromList(titleQualifiers));
			}

			return Regex.Replace(result, @"((^\w)|(\s|\p{P})\w)", match => match.Value.ToUpper());
		}

		private string GenerateFirstName(Gender gender)
		{
			return SimRandom.RandomFloat01() > 0.5f ? GenerateNameFromExisting(gender) : GenerateSyllabicName(minFirstNameSyllables, maxFirstNameSyllables);
		}

		private string GenerateSurname()
		{
			return GenerateSyllabicName(minSurnameSyllables, maxSurnameSyllables);
		}

		private string GenerateSyllabicName(int min, int max)
		{
			var startWithConsonant = SimRandom.RandomBool();
			var totalSounds = SimRandom.RandomRange(min, max + 1);
			if(!startWithConsonant && totalSounds < 3)
			{
				totalSounds = 3;
			}
			var result = string.Empty;

			for(int i = 0; i < totalSounds; i++)
			{
				if(startWithConsonant)
				{
					if (i == 0)
					{
						result += SimRandom.RandomEntryFromWeightedDictionary(beginningConsonants.dictionary);
					}
					else if (i == totalSounds - 1)
					{
						result += SimRandom.RandomEntryFromWeightedDictionary(endConsonants.dictionary);
					}
					else
					{
						result += SimRandom.RandomEntryFromWeightedDictionary(consonants.dictionary);
					}
				}
				else
				{
					if (i == 0)
					{
						result += SimRandom.RandomEntryFromWeightedDictionary(beginningVowels.dictionary);
					}
					else if (i == totalSounds - 1)
					{
						result += SimRandom.RandomEntryFromWeightedDictionary(endVowels.dictionary);
					}
					else
					{
						result += SimRandom.RandomEntryFromWeightedDictionary(vowels.dictionary);
					}
				}

				startWithConsonant = !startWithConsonant;
			}

			return result;
		}

		private string GenerateNameFromExisting(Gender gender)
		{
			var result = string.Empty;
			if(gender == Gender.MALE)
			{
				result = SimRandom.RandomEntryFromList(maleNames);
			}
			else
			{
				result = SimRandom.RandomEntryFromList(femaleNames);
			}

			var candidates = new List<char>() { 'a', 'e', 'i', 'o', 'u' };
			var toReplace = result.First(x => candidates.Contains(x));
			var regex = new Regex(Regex.Escape(toReplace.ToString()));
			result = regex.Replace(result, SimRandom.RandomEntryFromWeightedDictionary(vowels.dictionary), 1);

			return result;
		}
	}
}
