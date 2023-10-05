using Game.Enums;
using Game.Generators.Names;
using Game.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Game.Incidents
{
	public class FlavorService : SerializedMonoBehaviour
	{
		public static FlavorService Instance { get; private set; }

		public NamingThemePreset monsterPreset;

		//need to init with all of the flavor stuff like reasons
		public Dictionary<CreatureAlignment, List<string>> alignedReasons;
		public Dictionary<FlavorType, List<string>> flavorLists;
		public List<FlavorTemplateCollection> flavorTemplateCollections;

		private Dictionary<Type, FlavorTemplateCollection> flavorTemplateDictionary;

		public NamingTheme GenerateMonsterFactionNamingTheme()
		{
			var theme = new NamingTheme(monsterPreset);
			theme.consonants.RandomizeWeights(1, 10);
			theme.beginningConsonants.RandomizeWeights(1, 10);
			theme.endConsonants.RandomizeWeights(1, 10);
			theme.vowels.RandomizeWeights(1, 10);
			theme.beginningVowels.RandomizeWeights(1, 10);
			theme.endVowels.RandomizeWeights(1, 10);

			return theme;
		}

		public string GenerateFlavor(string phrase)
		{
			phrase = GenerateSynonyms(phrase);
			phrase = GenerateReasons(phrase);
			return phrase;
		}

		//unsure exactly where i left off with this part - we can now use synonyms but i need a way to cleanly
		//regex which matches to make instead of having separate fns for each and checking them all
		public string GenerateFlavor_2(string phrase)
		{
			var matches = Regex.Matches(phrase, @"\{([^\n \{\}]+):(GOOD|EVIL|LAWFUL|CHAOTIC)\}");
			foreach (Match match in matches)
			{
				var flavorTypeString = match.Groups[1].Value.ToString();
				if (Enum.TryParse<FlavorType>(flavorTypeString, out FlavorType flavorType))
				{
					var alignmentString = match.Groups[2].Value.ToString();
					if(Enum.TryParse<CreatureAlignment>(alignmentString, out CreatureAlignment alignment))
					{
						phrase = GenerateAlignmentBasedFlavor(flavorType, alignment, match.Value, phrase);
					}
				}
			}

			return phrase;
		}

		public string GetFlavorStringByType(Type type, OrganizationType priority, int goodEvilAxisAlignment, int lawfulChaoticAxisAlignment)
		{
			flavorTemplateDictionary.TryGetValue(type, out var collection);
			if(collection != null)
			{
				var matches = collection.GetMatches(priority, goodEvilAxisAlignment, lawfulChaoticAxisAlignment);
				if(matches.Count > 0)
				{
					return SimRandom.RandomEntryFromList(matches).flavor;
				}
				else
				{
					return "ERROR: NO MATCHING FLAVOR FOUND!";
				}
			}
			else
			{
				return "ERROR: NO MATCHING FLAVOR FOUND!";
			}
		}

		private string GenerateSynonyms(string phrase)
		{
			var matches = Regex.Matches(phrase, @"\{SYNONYM:([^\n \{\}]+)\}");

			foreach (Match match in matches)
			{
				var groupType = match.Groups[1].Value.ToString();
				if (ThesaurusProvider.Thesaurus.TryGetValue(match.Value, out List<string> synonyms))
				{
					var matchString = match.Value;
					var replaceString = SimRandom.RandomEntryFromList(synonyms);
					phrase = StringUtilities.ReplaceFirstOccurence(phrase, matchString, replaceString);
				}
			}
			return phrase;
		}

		private string GenerateAlignmentBasedFlavor(FlavorType flavorType, CreatureAlignment alignment, string match, string phrase)
		{
			var tempDict = new Dictionary<FlavorType, Dictionary<CreatureAlignment, List<string>>>();
			var flavor = SimRandom.RandomEntryFromList(tempDict[flavorType][alignment]);
			return StringUtilities.ReplaceFirstOccurence(phrase, match, flavor);
		}

		private string GenerateReasons(string phrase)
		{
			var matches = Regex.Matches(phrase, @"\{REASON:(GOOD|EVIL|LAWFUL|CHAOTIC)\}");

			foreach (Match match in matches)
			{
				var groupType = match.Groups[1].Value.ToString();
				if(Enum.TryParse<CreatureAlignment>(groupType, out CreatureAlignment result))
				{
					var matchString = match.Value;
					var replaceString = SimRandom.RandomEntryFromList(alignedReasons[result]);
					phrase = StringUtilities.ReplaceFirstOccurence(phrase, matchString, replaceString);
				}
			}
			return phrase;
		}

		private void LoadFlavorTemplates()
		{
			flavorTemplateDictionary = new Dictionary<Type, FlavorTemplateCollection>();
			foreach(var collection in flavorTemplateCollections)
			{
				if(collection.flavorTemplates != null && collection.flavorTemplates.Count > 0)
				{
					flavorTemplateDictionary.Add(collection.flavorTemplates.First().GetType(), collection);
				}
			}
		}

		public void Init()
		{
			if(Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
				LoadFlavorTemplates();
			}
		}
	}
}