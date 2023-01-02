using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Generators.Names
{
	[Serializable]
	public class NamingThemeCollection
	{
		public SerializedStringIntDictionary nouns;
		public SerializedStringIntDictionary verbs;
		public SerializedStringIntDictionary adjectives;

		public SerializedStringIntDictionary consonants;
		public SerializedStringIntDictionary vowels;
		public SerializedStringIntDictionary prepositions;

		public NamingThemeCollection() { }

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

			result.nouns = SerializedStringIntDictionary.Merge(a.nouns, b.nouns);
			result.verbs = SerializedStringIntDictionary.Merge(a.verbs, b.verbs);
			result.adjectives = SerializedStringIntDictionary.Merge(a.adjectives, b.adjectives);
			result.consonants = SerializedStringIntDictionary.Merge(a.consonants, b.consonants);
			result.vowels = SerializedStringIntDictionary.Merge(a.vowels, b.vowels);
			result.prepositions = SerializedStringIntDictionary.Merge(a.prepositions, b.prepositions);

			return result;
		}
	}
}
