using Game.Data;
using Game.Debug;
using Game.Enums;
using Game.Generators.Items;
using Game.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Data
{
	public class DataReader : EditorWindow
	{
		[MenuItem("Window/Data Reader")]
		public static void ShowWindow()
		{
			GetWindow<DataReader>("Data Reader");
		}

		private void OnGUI()
		{
			if (GUILayout.Button("Build Creatures"))
			{
				//BuildCreatures();
				NewBuildMonsters();
			}
			if (GUILayout.Button("Build Weapon Stats"))
			{
				//BuildWeaponStats();
			}
		}

		private int SavingThrowHelper(string input, string lookingFor)
		{
			if (!input.Contains(lookingFor))
			{
				return 0;
			}

			var split = input.Replace("Saving Throws ", "").Split(',');
			foreach (var s in split)
			{
				if (s.Contains(lookingFor))
				{
					var toParse = s.Replace(lookingFor, "").Replace(" ", "").Replace("+", "");
					int toReturn;
					if (int.TryParse(toParse, out toReturn))
					{
						return toReturn;
					}
				}
			}

			return 0;
		}

		private static Dictionary<string, CreatureSize> creatureSizes = new Dictionary<string, CreatureSize>()
		{
			{"T", CreatureSize.TINY}, {"S", CreatureSize.SMALL}, {"M", CreatureSize.MEDIUM},
			{"L", CreatureSize.LARGE}, {"H", CreatureSize.HUGE}, {"G", CreatureSize.GARGANTUAN}
		};

		private static Dictionary<string, CreatureType> creatureTypes = new Dictionary<string, CreatureType>()
		{
			{"ABERRATION", CreatureType.ABERRATION}, {"BEAST", CreatureType.BEAST}, {"CELESTIAL", CreatureType.CELESTIAL},
			{"CONSTRUCT", CreatureType.CONSTRUCT}, {"DRAGON", CreatureType.DRAGON}, {"FEY", CreatureType.FEY},
			{"FIEND", CreatureType.FIEND}, {"GIANT", CreatureType.GIANT}, {"HUMANOID", CreatureType.HUMANOID},
			{"MONSTROSITY", CreatureType.MONSTROSITY}, {"OOZE", CreatureType.OOZE}, {"PLANT", CreatureType.PLANT},
			{"UNDEAD", CreatureType.UNDEAD}, {"ELEMENTAL", CreatureType.ELEMENTAL}
		};

		private static Dictionary<string, CreatureAlignment> creatureAlignments = new Dictionary<string, CreatureAlignment>()
		{
			{"ANY ALIGNMENT", CreatureAlignment.ANY_ALIGNMENT}, {"CHAOTIC", CreatureAlignment.CHAOTIC}, {"CHAOTIC EVIL", CreatureAlignment.CHAOTIC_EVIL},
			{"CHAOTIC GOOD", CreatureAlignment.CHAOTIC_GOOD}, {"CHAOTIC NEUTRAL", CreatureAlignment.CHAOTIC_NEUTRAL}, {"EVIL", CreatureAlignment.EVIL},
			{"GOOD", CreatureAlignment.GOOD}, {"LAWFUL", CreatureAlignment.LAWFUL}, {"LAWFUL EVIL", CreatureAlignment.LAWFUL_EVIL},
			{"LAWFUL GOOD", CreatureAlignment.LAWFUL_GOOD}, {"LAWFUL NEUTRAL", CreatureAlignment.LAWFUL_NEUTRAL}, {"NEUTRAL EVIL", CreatureAlignment.NEUTRAL_EVIL},
			{"NEUTRAL GOOD", CreatureAlignment.NEUTRAL_GOOD}, {"NEUTRAL", CreatureAlignment.TRUE_NEUTRAL}, {"UNALIGNED", CreatureAlignment.UNALIGNED}
		};

		private static Dictionary<string, SensesType> creatureSenses = new Dictionary<string, SensesType>()
		{
			{"BLINDSIGHT", SensesType.BLINDSIGHT}, {"DARKVISION", SensesType.DARKVISION}, {"TREMORSENSE", SensesType.TREMORSENSE},
			{"TRUESIGHT", SensesType.TRUESIGHT}
		};

		private static Dictionary<string, float> creatureExp = new Dictionary<string, float>()
		{
			{"0", 10}, {"1/8", 25}, {"1/4", 50}, {"1/2", 100}, {"1", 200}, {"2", 450},
			{"3", 700}, {"4", 1100}, {"5", 1800}, {"6", 2300}, {"7", 2900}, {"8", 3900},
			{"9", 5000}, {"10", 5900}, {"11", 7200}, {"12", 8400}, {"13", 10000}, {"14", 11500},
			{"15", 13000}, {"16", 15000}, {"17", 18000}, {"18", 20000}, {"19", 22000}, {"20", 25000},
			{"21", 33000}, {"22", 41000}, {"23", 50000}, {"24", 62000}, {"30", 155000}
		};

		private void NewBuildMonsters()
		{
			var existingMonsters = Resources.LoadAll("ScriptableObjects/Monsters");
			var monsterDictionary = new Dictionary<string, MonsterData>();

			if (existingMonsters != null)
			{
				foreach (var c in existingMonsters)
				{
					monsterDictionary.Add(((MonsterData)c).Name, (MonsterData)c);		
				}
			}

			XDocument root = XDocument.Load("Assets/Resources/RawData/Monster Manual Bestiary.xml");
			var descendants = root.Descendants("monster").ToList();

			for (int i = 0; i < descendants.Count; i++)
			{
				var element = descendants[i];
				var monsterName = element.Element("name").Value;
				if(monsterName.Contains('/'))
				{
					var match = Regex.Match(monsterName, @"(.+ |)(\w+)\/(\w+)");
					var nameOne = match.Groups[1].Value + match.Groups[2].Value;
					var nameTwo = match.Groups[1].Value + match.Groups[3].Value;

					BuildMonster(element, nameOne, monsterDictionary);
					BuildMonster(element, nameTwo, monsterDictionary);
				}
				else
				{
					BuildMonster(element, monsterName, monsterDictionary);
				}
			}

			AssetDatabase.SaveAssets();
		}

		private void BuildMonster(XElement element, string monsterName, Dictionary<string, MonsterData> monsterDictionary)
		{
			MonsterData creatureStats;
			if (monsterDictionary.TryGetValue(monsterName, out var data))
			{
				creatureStats = data;
			}
			else
			{
				creatureStats = (MonsterData)CreateInstance(typeof(MonsterData));
				AssetDatabase.CreateAsset(creatureStats, "Assets/Resources/ScriptableObjects/Monsters/" + monsterName + ".asset");
			}

			creatureStats.name = monsterName;
			if (creatureSizes.TryGetValue(element.Element("size").Value, out var size))
			{
				creatureStats.size = size;
			}
			else
			{
				creatureStats.size = CreatureSize.TINY;
			}

			//Type
			var preSplitType = element.Element("type").Value.ToUpper();
			var postSplitTypeStrings = preSplitType.Split(',', ' ');
			var typeString = postSplitTypeStrings[0].ToUpper();
			if (preSplitType.Contains("HUMANOID"))
			{
				creatureStats.type = CreatureType.HUMANOID;
			}
			else if(preSplitType.Contains("SWARM OF"))
			{
				creatureStats.type = CreatureType.BEAST;
			}
			else
			{
				if (creatureTypes.TryGetValue(typeString, out var type))
				{
					creatureStats.type = type;
				}
				else
				{
					creatureStats.type = CreatureType.ABERRATION;
					OutputLogger.LogWarning("No Type Found for: " + creatureStats.name);
				}
			}

			//Alignment
			if (creatureAlignments.TryGetValue(element.Element("alignment").Value.ToUpper(), out var alignment))
			{
				creatureStats.alignment = alignment;
			}
			else
			{
				creatureStats.alignment = CreatureAlignment.ANY_ALIGNMENT;
			}

			//Armor
			var preSplitArmor = element.Element("ac").Value;
			var postSplitArmorStrings = preSplitArmor.Split(new char[] { ' ' }, 2);
			if (Int32.TryParse(postSplitArmorStrings[0], out var ac))
			{
				creatureStats.armorValue = ac;
			}

			if (postSplitArmorStrings.Length > 1)
			{
				creatureStats.armorType = postSplitArmorStrings[1];
			}

			//Hp
			var preSplitHealth = element.Element("hp").Value;
			var postSplitHealthStrings = preSplitHealth.Split(new char[] { ' ' }, 2);
			if (Int32.TryParse(postSplitHealthStrings[0], out var hp))
			{
				creatureStats.health = hp;
			}

			//Speeds
			var speedString = element.Element("speed").Value;
			var speedMatches = Regex.Matches(speedString, @"(\w+ |)(\d+) ft\.");

			foreach (Match speedMatch in speedMatches)
			{
				var firstGroup = speedMatch.Groups[1].Value;
				var secondGroup = speedMatch.Groups[2].Value;

				if (firstGroup == "climb ")
				{
					creatureStats.climbSpeed = GuaranteedIntParse(secondGroup);
				}
				else if (firstGroup == "swim ")
				{
					creatureStats.swimSpeed = GuaranteedIntParse(secondGroup);
				}
				else if (firstGroup == "fly ")
				{
					creatureStats.flySpeed = GuaranteedIntParse(secondGroup);
				}
				else
				{
					creatureStats.speed = GuaranteedIntParse(secondGroup);
				}
			}
			if(creatureStats.speed == 0 && creatureStats.climbSpeed == 0 && creatureStats.flySpeed == 0 && creatureStats.swimSpeed != 0)
			{
				creatureStats.landDwelling = false;
			}
			else
			{
				creatureStats.landDwelling = true;
			}

			//Stats
			var statBlock = new SerializableStatBlock();
			statBlock.strength = GuaranteedIntParse(element.Element("str").Value);
			statBlock.dexterity = GuaranteedIntParse(element.Element("dex").Value);
			statBlock.constitution = GuaranteedIntParse(element.Element("con").Value);
			statBlock.intelligence = GuaranteedIntParse(element.Element("int").Value);
			statBlock.wisdom = GuaranteedIntParse(element.Element("wis").Value);
			statBlock.charisma = GuaranteedIntParse(element.Element("cha").Value);
			statBlock.luck = 10;

			creatureStats.stats = statBlock;

			//Saves
			var saveBlock = new SerializableStatBlock();
			if (element.Descendants("save").Any())
			{
				var savesString = element.Element("save").Value;
				var savesMatches = Regex.Matches(savesString, @"([A-z]{3}) (\+\d+|-[\d]+)");
				foreach (Match saveMatch in savesMatches)
				{
					var statString = saveMatch.Groups[1].Value.ToUpper();
					var number = GuaranteedIntParse(saveMatch.Groups[2].Value);
					if (statString == "STR")
					{
						saveBlock.strength = number;
					}
					else if (statString == "DEX")
					{
						saveBlock.dexterity = number;
					}
					else if (statString == "CON")
					{
						saveBlock.constitution = number;
					}
					else if (statString == "INT")
					{
						saveBlock.intelligence = number;
					}
					else if (statString == "WIS")
					{
						saveBlock.wisdom = number;
					}
					else if (statString == "CHA")
					{
						saveBlock.charisma = number;
					}
					else
					{
						OutputLogger.LogError("Unknown stat string discovered!");
					}
				}

				creatureStats.savingThrows = saveBlock;
			}
			else
			{
				OutputLogger.Log("No saves found for: " + monsterName);
			}

			if (element.Descendants("skill").Any())
			{
				var skillsList = new List<string>();
				var skillsString = element.Element("skill").Value;
				var skillsMatches = Regex.Matches(skillsString, @"([A-z]+ (\+\d+|-[\d]+))");
				foreach (Match skillsMatch in skillsMatches)
				{
					skillsList.Add(skillsMatch.Groups[1].Value);
				}

				creatureStats.skills = skillsList;
			}

			if (element.Descendants("senses").Any())
			{
				var sensesList = new List<SenseRange>();
				var sensesString = element.Element("senses").Value;
				var sensesMatches = Regex.Matches(sensesString, @"(\w+ )(\d+) ft\.");
				foreach (Match sensesMatch in sensesMatches)
				{
					var senseString = sensesMatch.Groups[1].Value;
					var senseAmountString = sensesMatch.Groups[2].Value;
					if (creatureSenses.TryGetValue(senseString.ToUpper(), out var sense))
					{
						var senseRange = new SenseRange { senseType = sense, distance = GuaranteedIntParse(senseAmountString) };
						sensesList.Add(senseRange);
					}
				}

				creatureStats.senses = sensesList;
			}

			if (element.Descendants("passive").Any())
			{
				creatureStats.passivePerception = GuaranteedIntParse(element.Element("passive").Value);
			}

			if (element.Descendants("immune").Any())
			{
				var damageTypeList = new List<DamageType>();
				var value = element.Element("immune").Value;
				var matches = Regex.Matches(value, @"(\w+)");
				foreach (Match match in matches)
				{
					var matchValue = match.Groups[1].Value;
					if (Enum.TryParse(matchValue.ToUpper(), out DamageType type))
					{
						damageTypeList.Add(type);
					}
				}

				creatureStats.damageImmunities = damageTypeList;
			}

			if (element.Descendants("vulnerable").Any())
			{
				var damageTypeList = new List<DamageType>();
				var value = element.Element("vulnerable").Value;
				var matches = Regex.Matches(value, @"(\w+)");
				foreach (Match match in matches)
				{
					var matchValue = match.Groups[1].Value;
					if (Enum.TryParse(matchValue.ToUpper(), out DamageType type))
					{
						damageTypeList.Add(type);
					}
				}

				creatureStats.damageVulnerabilities = damageTypeList;
			}

			if (element.Descendants("resist").Any())
			{
				var damageTypeList = new List<DamageType>();
				var value = element.Element("resist").Value;
				var matches = Regex.Matches(value, @"(\w+)");
				foreach (Match match in matches)
				{
					var matchValue = match.Groups[1].Value;
					if (Enum.TryParse(matchValue.ToUpper(), out DamageType type))
					{
						damageTypeList.Add(type);
					}
				}

				creatureStats.damageResistances = damageTypeList;
			}

			if (element.Descendants("conditionImmune").Any())
			{
				var conditionTypeList = new List<ConditionType>();
				var value = element.Element("conditionImmune").Value;
				var matches = Regex.Matches(value, @"(\w+)");
				foreach (Match match in matches)
				{
					var matchValue = match.Groups[1].Value;
					if (Enum.TryParse(matchValue.ToUpper(), out ConditionType type))
					{
						conditionTypeList.Add(type);
					}
				}

				creatureStats.conditionImmunities = conditionTypeList;
			}

			if (element.Descendants("languages").Any())
			{
				var languagesList = new List<string>();
				var languagesString = element.Element("languages").Value;

				var languagesSplit = languagesString.Split(',');
				foreach (var languageString in languagesSplit)
				{
					languagesList.Add(languageString.Trim());
				}
				creatureStats.languages = languagesList;
			}

			creatureStats.challengeRating = GuaranteedFloatParse(element.Element("cr").Value);
			if (creatureExp.TryGetValue(element.Element("cr").Value, out var exp))
			{
				creatureStats.experienceYield = exp;
			}

			if (element.Descendants("trait").Any())
			{
				var abilities = new List<CreatureAbility>();
				foreach (var trait in element.Descendants("trait"))
				{
					var ability = new CreatureAbility();
					ability.abilityName = trait.Element("name").Value;
					ability.abilityDescription = trait.Element("text").Value;
					abilities.Add(ability);
				}
			}

			if (element.Descendants("action").Any())
			{
				var actions = new List<CreatureAction>();
				foreach (var trait in element.Descendants("action"))
				{
					var action = new CreatureAction();
					action.actionName = trait.Element("name").Value;
					foreach (var actionText in trait.Elements("text"))
					{
						action.actionDescription += (actionText.Value + '\n');
					}
					action.actionDescription.TrimEnd('\r', '\n');
					actions.Add(action);
				}
				creatureStats.actions = actions;
			}

			if (element.Descendants("legendary").Any())
			{
				var actions = new List<CreatureAction>();
				foreach (var trait in element.Descendants("legendary"))
				{
					var action = new CreatureAction();
					action.actionName = trait.Element("name").Value;
					foreach (var actionText in trait.Elements("text"))
					{
						action.actionDescription += (actionText.Value + '\n');
					}
					action.actionDescription.TrimEnd('\r', '\n');
					actions.Add(action);
				}
				creatureStats.legendaryActions = actions;
				creatureStats.legendary = true;
			}
		}

		private int GuaranteedIntParse(string toParse)
		{
			if(Int32.TryParse(toParse, out var parsed))
			{
				return parsed;
			}
			else
			{
				return 0;
			}
		}

		private float GuaranteedFloatParse(string toParse)
		{
			if(toParse == "1/8")
			{
				return 0.125f;
			}
			else if(toParse == "1/4")
			{
				return 0.25f;
			}
			else if(toParse == "1/2")
			{
				return 0.5f;
			}
			else if (float.TryParse(toParse, out var parsed))
			{
				return parsed;
			}
			else
			{
				return 0;
			}
		}
	}
}