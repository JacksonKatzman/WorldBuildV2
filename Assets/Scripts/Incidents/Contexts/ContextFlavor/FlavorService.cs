using Game.Enums;
using Game.Generators.Names;
using Game.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Incidents
{
	public class FlavorService : SerializedMonoBehaviour
	{
		public static FlavorService Instance { get; private set; }

		public NamingThemePreset monsterPreset;

		//need to init with all of the flavor stuff like reasons
		public Dictionary<CreatureAlignment, List<string>> alignedReasons;

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
			var final = GenerateReasons(phrase);
			return final;
		}

		private string GenerateReasons(string phrase)
		{
			var matches = Regex.Matches(phrase, @"\{R:(GOOD|EVIL|LAWFUL|CHAOTIC)\}");

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

		public void Init()
		{
			if(Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
			}
		}
	}
}