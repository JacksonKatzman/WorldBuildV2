using Game.Enums;
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

		//need to init with all of the flavor stuff like reasons
		public Dictionary<CreatureAlignment, List<string>> alignedReasons;

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
				var groupType = match.Groups[0].Value.ToString();
				if(Enum.TryParse<CreatureAlignment>(groupType, out CreatureAlignment result))
				{
					var matchString = match.Value;
					var replaceString = SimRandom.RandomEntryFromList(alignedReasons[result]);
					phrase = StringUtilities.ReplaceFirstOccurence(phrase, matchString, replaceString);
				}
			}
			return phrase;
		}

		private void Awake()
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