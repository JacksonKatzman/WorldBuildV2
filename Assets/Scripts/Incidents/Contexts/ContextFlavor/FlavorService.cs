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
			return phrase;
		}

		/*
		Ideas for flavor tags/types to add for the future:
		{BodyPart}, {Color}, 
		*/

		public bool GetFlavorTemplateByType(Type type, OrganizationType priority, int goodEvilAxisAlignment, int lawfulChaoticAxisAlignment, out AbstractFlavorTemplate template)
		{
			template = null;

			flavorTemplateDictionary.TryGetValue(type, out var collection);
			if(collection != null)
			{
				var matches = collection.GetMatches(priority, goodEvilAxisAlignment, lawfulChaoticAxisAlignment);
				if(matches.Count > 0)
				{
					template = SimRandom.RandomEntryFromList(matches);
					return true;
				}
				else
				{
					return false;
				}
			}
			else
			{
				return false;
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