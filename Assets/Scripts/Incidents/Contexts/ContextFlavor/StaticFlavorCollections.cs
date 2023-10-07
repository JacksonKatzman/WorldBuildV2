using Game.Enums;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Game.Incidents
{
	public static class StaticFlavorCollections
	{
		public static string OBJECT_PRONOUN_STRING = "OBJP";
		public static string SUBJECT_PRONOUN_STRING = "SUBP";
		public static string POSSESSIVE_PRONOUN_STRING = "POSP";

		public static Dictionary<string, Func<string, Dictionary<string, IIncidentContext>, string>> flavorFunctions = new Dictionary<string, Func<string, Dictionary<string, IIncidentContext>, string>>()
		{
			{OBJECT_PRONOUN_STRING, HandleObjectGenderPronouns}, {SUBJECT_PRONOUN_STRING, HandleSubjectGenderPronouns}, {POSSESSIVE_PRONOUN_STRING, HandlePossessiveGenderPronouns}
		};

		public static string InjectFlavor(string input, Dictionary<string, IIncidentContext> contexts)
		{
			var textLine = string.Copy(input);
			var matches = Regex.Matches(textLine, @"\{(\w+):");

			foreach(Match match in matches)
			{
				var tag = match.Groups[1].Value;
				if(flavorFunctions.TryGetValue(tag, out var function))
				{
					textLine = function(textLine, contexts);
				}
			}

			return textLine;
		}

		public static string HandleObjectGenderPronouns(string input, Dictionary<string, IIncidentContext> contexts)
		{
			var textLine = string.Copy(input);
			var matches = Regex.Matches(textLine, @"\{" + OBJECT_PRONOUN_STRING + @":(\d+)\}");

			foreach (Match match in matches)
			{
				var matchString = match.Value;
				var matchId = match.Groups[1].Value;
				var keyString = "{" + matchId + "}";
				var linkedContext = contexts[keyString];

				var pronounString = "OBJECT-PRONOUN";
				if (linkedContext.GetType() == typeof(Character))
				{
					var character = linkedContext as Character;
					pronounString = StaticFlavorCollections.objectGenderPronouns[character.Gender];
				}
				else
				{
					pronounString = "it";
				}

				var linkString = string.Format("<link=\"{0}\">{1}</link>", linkedContext.ID, pronounString);
				textLine = textLine.Replace(matchString, linkString);
			}

			return textLine;
		}

		public static string HandleSubjectGenderPronouns(string input, Dictionary<string, IIncidentContext> contexts)
		{
			var textLine = string.Copy(input);
			var matches = Regex.Matches(textLine, @"\{" + SUBJECT_PRONOUN_STRING + @":(\d+)\}");

			foreach (Match match in matches)
			{
				var matchString = match.Value;
				var matchId = match.Groups[1].Value;
				var keyString = "{" + matchId + "}";
				var linkedContext = contexts[keyString];

				var pronounString = "SUBJECT-PRONOUN";
				if (linkedContext.GetType() == typeof(Character))
				{
					var character = linkedContext as Character;
					pronounString = StaticFlavorCollections.subjectGenderPronouns[character.Gender];
				}
				else
				{
					pronounString = "it";
				}

				var linkString = string.Format("<link=\"{0}\">{1}</link>", linkedContext.ID, pronounString);
				textLine = textLine.Replace(matchString, linkString);
			}

			return textLine;
		}

		public static string HandlePossessiveGenderPronouns(string input, Dictionary<string, IIncidentContext> contexts)
		{
			var textLine = string.Copy(input);
			var matches = Regex.Matches(textLine, @"\{" + POSSESSIVE_PRONOUN_STRING + @":(\d+)\}");

			foreach (Match match in matches)
			{
				var matchString = match.Value;
				var matchId = match.Groups[1].Value;
				var keyString = "{" + matchId + "}";
				var linkedContext = contexts[keyString];

				var pronounString = "POS-PRONOUN";
				if (linkedContext.GetType() == typeof(Character))
				{
					var character = linkedContext as Character;
					pronounString = StaticFlavorCollections.possessiveGenderPronouns[character.Gender];
				}
				else
				{
					pronounString = "its";
				}

				var linkString = string.Format("<link=\"{0}\">{1}</link>", linkedContext.ID, pronounString);
				textLine = textLine.Replace(matchString, linkString);
			}

			return textLine;
		}

		public static Dictionary<Gender, string> objectGenderPronouns = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "him"}, {Gender.FEMALE, "her"}, {Gender.ANY, "them"}
		};

		public static Dictionary<Gender, string> subjectGenderPronouns = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "he"}, {Gender.FEMALE, "she"}, {Gender.ANY, "they"}
		};

		public static Dictionary<Gender, string> possessiveGenderPronouns = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "his"}, {Gender.FEMALE, "her"}, {Gender.ANY, "their"}
		};
	}
}