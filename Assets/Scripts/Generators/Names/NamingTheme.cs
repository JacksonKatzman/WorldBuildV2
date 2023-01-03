using Game.Data;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;

namespace Game.Generators.Names
{
	public class NamingTheme
	{
		private NamingThemeCollection collection;
		public int averageFirstNameSyllables;
		public int averageSurnameSyllables;
		public WeightedListSerializableDictionary<string> personNameFormats;
		public WeightedListSerializableDictionary<string> surnameFormats;

		public NamingTheme(NamingThemePreset preset)
		{
			collection = new NamingThemeCollection(preset.themeCollections[0].collection);

			for(int i = 1; i < preset.themeCollections.Count; i++)
			{
				collection = collection + preset.themeCollections[i].collection;
			}

			averageFirstNameSyllables = preset.averageFirstNameSyllables;
			averageSurnameSyllables = preset.averageSurnameSyllables;
			surnameFormats = preset.surnameFormats;
		}

		public string GenerateName<Person>()
		{
			var format = SimRandom.RandomEntryFromWeightedDictionary(personNameFormats);

			return FillOutFormat(format);
		}

		private string FillOutFormat(string format)
		{
			var result = format;
			result = result.Replace("{F}", GenerateSyllabicName(averageFirstNameSyllables))
				.Replace("{S}", GenerateSurname())
				.Replace("{P}", SimRandom.RandomEntryFromWeightedDictionary(collection.prepositions))
				.Replace("{A}", SimRandom.RandomEntryFromWeightedDictionary(collection.adjectives))
				.Replace("{V}", SimRandom.RandomEntryFromWeightedDictionary(collection.verbs))
				.Replace("{N}", SimRandom.RandomEntryFromWeightedDictionary(collection.nouns));

			return result;
		}

		private string GenerateSurname()
		{
			var format = SimRandom.RandomEntryFromWeightedDictionary(surnameFormats);
			format = format.Replace("{S}", GenerateSyllabicName(averageSurnameSyllables));
			return FillOutFormat(format);
		}

		private string GenerateSyllabicName(int averageSyllables)
		{
			var startWithConsonant = SimRandom.RandomBool();
			var halfAverage = averageSyllables / 2;
			var totalSounds = SimRandom.RandomRange(averageSyllables - halfAverage, averageSyllables + halfAverage + 1) * 2;
			var result = string.Empty;

			for(int i = 0; i < totalSounds; i++)
			{
				if(startWithConsonant)
				{
					result += SimRandom.RandomEntryFromWeightedDictionary(collection.consonants);
				}
				else
				{
					result += SimRandom.RandomEntryFromWeightedDictionary(collection.vowels);
				}

				startWithConsonant = !startWithConsonant;
			}

			return result;
		}
	}

	static public class NameGenerator
	{
		//static public string GenerateName<Person>()
	}
}
