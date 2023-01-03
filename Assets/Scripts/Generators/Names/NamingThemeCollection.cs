using Game.Data;
using System;
using System.Collections.Generic;

namespace Game.Generators.Names
{
	[Serializable]
	public class NamingThemeCollection
	{
		public WeightedListSerializableDictionary<string> nouns;
		public WeightedListSerializableDictionary<string> verbs;
		public WeightedListSerializableDictionary<string> adjectives;

		public WeightedListSerializableDictionary<string> consonants;
		public WeightedListSerializableDictionary<string> vowels;
		public WeightedListSerializableDictionary<string> prepositions;

		public NamingThemeCollection() 
		{
			nouns = new WeightedListSerializableDictionary<string>();
			verbs = new WeightedListSerializableDictionary<string>();
			adjectives = new WeightedListSerializableDictionary<string>();
			consonants = new WeightedListSerializableDictionary<string>();
			vowels = new WeightedListSerializableDictionary<string>();
			prepositions = new WeightedListSerializableDictionary<string>();
		}

		public NamingThemeCollection(NamingThemeCollection copy)
		{
			nouns = copy.nouns;
			verbs = copy.verbs;
			adjectives = copy.adjectives;

			consonants = copy.consonants;
			vowels = copy.vowels;
			prepositions = copy.prepositions;
		}

		public static NamingThemeCollection operator + (NamingThemeCollection a, NamingThemeCollection b)
		{
			var result = new NamingThemeCollection();

			result.nouns = WeightedListSerializableDictionary<string>.Merge(a.nouns, b.nouns);
			result.verbs = WeightedListSerializableDictionary<string>.Merge(a.verbs, b.verbs);
			result.adjectives = WeightedListSerializableDictionary<string>.Merge(a.adjectives, b.adjectives);
			result.consonants = WeightedListSerializableDictionary<string>.Merge(a.consonants, b.consonants);
			result.vowels = WeightedListSerializableDictionary<string>.Merge(a.vowels, b.vowels);
			result.prepositions = WeightedListSerializableDictionary<string>.Merge(a.prepositions, b.prepositions);

			return result;
		}
	}
}
