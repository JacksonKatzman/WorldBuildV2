using Game.Debug;
using Game.Enums;
using Game.Utilities;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Game.Incidents
{
	public static class StaticFlavorCollections
	{
		public static string OBJECT_PRONOUN_STRING = "OBJP";
		public static string SUBJECT_PRONOUN_STRING = "SUBP";
		public static string POSSESSIVE_PRONOUN_STRING = "POSP";
		public static string CHARACTER_RELATIONSHIP_STRING = "RELATE";
		public static string SYNONYM_STRING = "SYNONYM";
		public static string FULL_TITLE_STRING = "TITLED";
		public static string PREVIOUS_TITLE_STRING = "PREVTITLED";

		public static Dictionary<string, Func<string, Dictionary<string, IIncidentContext>, string>> flavorFunctions = new Dictionary<string, Func<string, Dictionary<string, IIncidentContext>, string>>()
		{
			{OBJECT_PRONOUN_STRING, HandleObjectGenderPronouns}, {SUBJECT_PRONOUN_STRING, HandleSubjectGenderPronouns}, {POSSESSIVE_PRONOUN_STRING, HandlePossessiveGenderPronouns},
			{CHARACTER_RELATIONSHIP_STRING, HandleCharacterRelationshipTitles}, {SYNONYM_STRING, GenerateSynonyms}, {FULL_TITLE_STRING, HandleCharacterFullTitles},
			{PREVIOUS_TITLE_STRING, HandleCharacterPreviousTitles}
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

		public static string GenerateSynonyms(string input, Dictionary<string, IIncidentContext> contexts)
		{
			var textLine = string.Copy(input);
			var matches = Regex.Matches(textLine, @"\{" + SYNONYM_STRING + @":([^\n \{\}]+)\}");

			foreach (Match match in matches)
			{
				var groupType = match.Groups[1].Value.ToString();
				if (ThesaurusProvider.Thesaurus.TryGetValue(match.Value, out List<string> synonyms))
				{
					var matchString = match.Value;
					var replaceString = SimRandom.RandomEntryFromList(synonyms);
					textLine = StringUtilities.ReplaceFirstOccurence(textLine, matchString, replaceString);
				}
			}
			return textLine;
		}

		public static string HandleCharacterFullTitles(string input, Dictionary<string, IIncidentContext> contexts)
		{
			var textLine = string.Copy(input);
			var matches = Regex.Matches(textLine, @"\{" + FULL_TITLE_STRING + @":(\d+)\}");

			foreach (Match match in matches)
			{
				var matchString = match.Value;
				var matchId = match.Groups[1].Value;
				var keyString = "{" + matchId + "}";
				var linkedContext = contexts[keyString];

				var titleString = "FULL-TITLE";
				if (linkedContext.GetType() == typeof(Character))
				{
					var character = linkedContext as Character;
					titleString = character.CharacterName.GetTitledFullName(character);
				}
				else
				{
					titleString = linkedContext.Name;
				}

				var linkString = string.Format("<link=\"{0}\">{1}</link>", linkedContext.ID, titleString);
				textLine = textLine.Replace(matchString, linkString);
			}

			return textLine;
		}

		public static string HandleCharacterPreviousTitles(string input, Dictionary<string, IIncidentContext> contexts)
		{
			var textLine = string.Copy(input);
			var matches = Regex.Matches(textLine, @"\{" + PREVIOUS_TITLE_STRING + @":(\d+)\}");

			foreach (Match match in matches)
			{
				var matchString = match.Value;
				var matchId = match.Groups[1].Value;
				var keyString = "{" + matchId + "}";
				var linkedContext = contexts[keyString];

				var titleString = "PREVIOUS-TITLE";
				if (linkedContext.GetType() == typeof(Character))
				{
					var character = linkedContext as Character;
					titleString = character.CharacterName.GetPreviousTitledFullName(character);
				}
				else
				{
					titleString = linkedContext.Name;
				}

				var linkString = string.Format("<link=\"{0}\">{1}</link>", linkedContext.ID, titleString);
				textLine = textLine.Replace(matchString, linkString);
			}

			return textLine;
		}

		public static string HandleCharacterRelationshipTitles(string input, Dictionary<string, IIncidentContext> contexts)
		{
			var textLine = string.Copy(input);
			var matches = Regex.Matches(textLine, @"\{" + CHARACTER_RELATIONSHIP_STRING + @":(\d+)/(\d+)\}");
			
			if(matches.Count == 0)
			{
				OutputLogger.LogError("HandleFamilyRelationshipTitles has incorrect input.");
			}

			foreach(Match match in matches)
			{
				var relationshipString = "relative";

				var matchString = match.Value;
				var primaryCharacterId = "{" + match.Groups[1].Value + "}";
				var secondaryCharacterId = "{" + match.Groups[2].Value + "}";

				var primaryCharacterContext = contexts[primaryCharacterId];
				var secondaryCharacterContext = contexts[secondaryCharacterId];

				if(primaryCharacterContext.GetType() != typeof(Character) || secondaryCharacterContext.GetType() != typeof(Character))
				{
					relationshipString = "thing";
					OutputLogger.LogError("Non-Character context entered into HandleFamilyRelationshipTitles!");
				}
				else
				{
					var primary = primaryCharacterContext as Character;
					var secondary = secondaryCharacterContext as Character;

					var data = CharacterExtensions.FindRelationship(primary, secondary);
					if(data != null)
					{
						var oneWayPath = (Mathf.Abs(data.verticallyRemoved) + data.spousallyRemoved + data.siblingallyRemoved) == data.iterations;
						if (oneWayPath)
						{
							var key = data.finalValue + (10 * data.spousallyRemoved) + (100 * data.siblingallyRemoved);
							key = data.verticallyRemoved >= 0 ? key + (1000 * data.verticallyRemoved) : (key * -1) + (1000* data.verticallyRemoved);
							if(relationshipTitles.TryGetValue(key, out var pair))
							{
								relationshipString = pair[secondary.Gender];
							}
							else
							{
								relationshipString = "relative";
							}
						}
						else
						{
							relationshipString = "relative";
						}
					}
					else
					{
						relationshipString = "acquaintance";
					}
				}

				var linkString = string.Format("<link=\"{0}\">{1}</link>", secondaryCharacterContext.ID, relationshipString);
				textLine = textLine.Replace(matchString, linkString);
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

		public static Dictionary<Gender, string> characterParentTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "father"}, {Gender.FEMALE, "mother"}, {Gender.ANY, "parent"}
		};

		public static Dictionary<Gender, string> characterChildrenTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "son"}, {Gender.FEMALE, "daughter"}, {Gender.ANY, "child"}
		};

		public static Dictionary<Gender, string> characterSiblingTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "brother"}, {Gender.FEMALE, "sister"}, {Gender.ANY, "sibling"}
		};

		public static Dictionary<Gender, string> characterSpouseTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "husband"}, {Gender.FEMALE, "wife"}, {Gender.ANY, "spouse"}
		};

		public static Dictionary<Gender, string> characterChildInLawTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "son in law"}, {Gender.FEMALE, "daughter in law"}, {Gender.ANY, "in law"}
		};

		public static Dictionary<Gender, string> characterStepChildTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "step-son"}, {Gender.FEMALE, "step-daughter"}, {Gender.ANY, "step-child"}
		};

		public static Dictionary<Gender, string> characterExSpouseTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "ex-husband"}, {Gender.FEMALE, "ex-wife"}, {Gender.ANY, "ex-spouse"}
		};

		public static Dictionary<Gender, string> characterGrandChildTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "grandson"}, {Gender.FEMALE, "granddaughter"}, {Gender.ANY, "grandchild"}
		};

		public static Dictionary<Gender, string> characterStepParentTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "step-father"}, {Gender.FEMALE, "step-mother"}, {Gender.ANY, "step-parent"}
		};

		public static Dictionary<Gender, string> characterUncleAuntTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "uncle"}, {Gender.FEMALE, "aunt"}, {Gender.ANY, "relative"}
		};

		public static Dictionary<Gender, string> characterGrandParentTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "grandfather"}, {Gender.FEMALE, "grandmother"}, {Gender.ANY, "grandparent"}
		};

		public static Dictionary<Gender, string> characterStepSiblingTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "step-brother"}, {Gender.FEMALE, "step-sister"}, {Gender.ANY, "step-sibling"}
		};

		public static Dictionary<Gender, string> characterNephewNieceTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "nephew"}, {Gender.FEMALE, "niece"}, {Gender.ANY, "neiphling"}
		};

		public static Dictionary<Gender, string> characterCousinTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "cousin"}, {Gender.FEMALE, "cousin"}, {Gender.ANY, "cousin"}
		};

		public static Dictionary<Gender, string> characterSiblingInLawTitles = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "brother in law"}, {Gender.FEMALE, "sister in law"}, {Gender.ANY, "relative"}
		};

		public static Dictionary<int, Dictionary<Gender, string>> relationshipTitles = new Dictionary<int, Dictionary<Gender, string>>()
		{
			{0001, characterSpouseTitles}, {0002, characterSiblingTitles}, {0003, characterParentTitles}, {0004, characterChildrenTitles},
			{1001, characterChildInLawTitles}, {1002, characterStepChildTitles}, {1003, characterExSpouseTitles}, {1004, characterGrandChildTitles},
			{-1001, characterStepParentTitles}, {-1002, characterUncleAuntTitles}, {-1003, characterGrandParentTitles}, {-1004, characterStepSiblingTitles},
			{-2001, characterGrandParentTitles}, {-2002, characterUncleAuntTitles}, {-2003, characterGrandParentTitles}, {-2004, characterUncleAuntTitles},
			{0101, characterSiblingInLawTitles}, {0104, characterNephewNieceTitles}, {-1104, characterCousinTitles}
		};
		// 1 - Spouse
		// 2 - Sibling
		// 3 - Parent
		// 4 - Child
		/*
		 * +0001 = Wife/Husband
		 * +0002 = Brother/Sister
		 * +0003 = Father/Mother
		 * +0004 = Son/Daughter
		 * +1001 = Son/Daugher In Law
		 * +1002 = Step Son/Daughter
		 * +1003 = Ex-Husband/Wife
		 * +1004 = Grand Son/Daughter
		 * -1001 = Step father/mother
		 * -1002 = Uncle/Aunt
		 * -1003 = Grand father/mother
		 * -1004 = Step brother/sister
		 * -2001 = Grand father/mother
		 * -2002 = Great Uncle/Aunt
		 * -2003 = Great Grand father/mother
		 * -2004 = Uncle/Aunt
		 * 
		 * +0101 = Brother/Sister In Law
		 * +0102 = N/A (Siblings sibling)
		 * +0103 = N/A
		 * +0104 = Nephew/Niece
		 * 
		 * IGNORE:
		 * +1101 = Son/Daugher In Law
		 * +1102 = N/A
		 * +1103 = N/A
		 * +1104 = Grand Son/Daughter
		 * 
		 * +0111 = Brother/Sister in Law?
		 * +0112 = Brother/Sister in Law?
		 * +0113 = Uncle/Aunt in law?
		 * +0114 = Step Nephew/Niece
		 * 
		 * +1111 = extended family member
		 * +1112 = extended family member
		 * 
		 * -1101 = Uncle/Aunt in law?
		 * -1102 = Uncle/Aunt
		 * -1103 = Step Grand Father/Mother
		 * -1104 = Cousin
		*/
	}
}