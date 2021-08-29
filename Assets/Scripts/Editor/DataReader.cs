using Game.Creatures;
using Game.Enums;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Editor
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
			if(GUILayout.Button("Build Creatures"))
			{
				BuildCreatures();
			}
		}

		private int SavingThrowHelper(string input, string lookingFor)
		{
			if(!input.Contains(lookingFor))
			{
				return 0;
			}

			var split = input.Replace("Saving Throws ", "").Split(',');
			foreach(var s in split)
			{
				if(s.Contains(lookingFor))
				{
					var toParse = s.Replace(lookingFor, "").Replace(" ", "").Replace("+", "");
					int toReturn;
					if(Int32.TryParse(toParse, out toReturn))
					{
						return toReturn;
					}
				}
			}

			return 0;
		}

		private void BuildCreatures()
		{
			var rawCreatureData = Resources.LoadAll("Creatures/RawData");
			var existingCreatures = Resources.LoadAll("Creatures/Creatures");
			var creatureDictionary = new Dictionary<string, CreatureStats>();

			if (existingCreatures != null)
			{
				foreach (var c in existingCreatures)
				{
					creatureDictionary.Add(((CreatureStats)c).Name, ((CreatureStats)c));
				}
			}
			if (rawCreatureData != null)
			{
				foreach (var data in rawCreatureData)
				{
					var trimmed = ((TextAsset)data).text.Replace("\r", "");
					var creatureDatas = trimmed.Split('@');

					foreach(var creature in creatureDatas)
					{
						var creatureLines = creature.Split('\n');
						var dataLines = new List<string>();
						foreach(var line in creatureLines)
						{
							if(line != string.Empty)
							{
								dataLines.Add(line);
							}
						}

						if(dataLines.Count == 0 || creatureDictionary.ContainsKey(dataLines[0]))
						{
							continue;
						}

						var creatureStats = (CreatureStats)ScriptableObject.CreateInstance(typeof(CreatureStats));

						var assetName = dataLines[0].Replace('/', '-');
						AssetDatabase.CreateAsset(creatureStats, "Assets/Resources/Creatures/Creatures/" + assetName + ".asset");

						creatureStats.name = dataLines[0];

						creatureDictionary.Add(creatureStats.name, creatureStats);

						dataLines[1] = Regex.Replace(dataLines[1], @"\(.*\)", "");

						var split1 = dataLines[1].Split(',');
						var split2 = split1[0].Split(' ');
						var align = split1[1].Trim(' ').Replace(' ', '_').ToUpper();
						if(align == "NEUTRAL")
						{
							align = "TRUE_NEUTRAL";
						}

						creatureStats.size = (CreatureSize)Enum.Parse(typeof(CreatureSize), split2[0].ToUpper());
						creatureStats.type = (CreatureType)Enum.Parse(typeof(CreatureType), split2[1].ToUpper());
						creatureStats.alignment = (CreatureAlignment)Enum.Parse(typeof(CreatureAlignment), align.ToUpper());
						creatureStats.stats = new PersonStats(Int32.Parse(dataLines[6].Split(' ')[0]), Int32.Parse(dataLines[8].Split(' ')[0]),
							Int32.Parse(dataLines[10].Split(' ')[0]), Int32.Parse(dataLines[12].Split(' ')[0]),
							Int32.Parse(dataLines[14].Split(' ')[0]), Int32.Parse(dataLines[16].Split(' ')[0]), 10);

						var abilitiesIndex = -1;
						var actionsIndex = -1;
						var legendaryIndex = -1;

						foreach(var line in dataLines)
						{
							if(line.StartsWith("Challenge"))
							{
								abilitiesIndex = dataLines.IndexOf(line);
							}
							if(line.StartsWith("ACTIONS"))
							{
								actionsIndex = dataLines.IndexOf(line);
							}
							if (line.StartsWith("LEGENDARY ACTIONS"))
							{
								legendaryIndex = dataLines.IndexOf(line);
								creatureStats.legendary = true;
							}

							if (line.StartsWith("Armor Class"))
							{
								creatureStats.armorValue = line.Replace("Armor Class ", "");
							}
							if (line.StartsWith("Hit Points"))
							{
								creatureStats.health = line.Replace("Hit Points ", "");
							}
							if (line.StartsWith("Speed"))
							{
								creatureStats.speed = line.Replace("Speed ", "");
								creatureStats.landDwelling = (!line.Contains("swim") || line.Contains("fly"));
							}
							if (line.StartsWith("Senses"))
							{
								creatureStats.senses = line.Replace("Senses ", "");
							}
							if (line.StartsWith("Skills"))
							{
								var skillsSplit = line.Replace("Skills ", "").Split(',');
								creatureStats.skills = new List<string>();
								foreach (var s in skillsSplit)
								{
									creatureStats.skills.Add(s);
								}
							}
							if (line.StartsWith("Languages"))
							{
								var split = line.Replace("Languages ", "").Split(',');
								creatureStats.languages = new List<string>();
								foreach (var s in split)
								{
									creatureStats.languages.Add(s);
								}
							}
							if (line.StartsWith("Damage Resistances"))
							{
								creatureStats.damageResistances = new List<DamageType>();
								var preSplit = line.Replace("Damage Resistances ", "");
								if (preSplit.Contains("nonmagical"))
								{
									creatureStats.damageResistances.Add(DamageType.NONMAGICAL);
									preSplit = preSplit.Replace("bludgeoning", "").Replace("slashing", "").Replace("piercing", "");
								}

								var split = preSplit.Split(',');

								foreach (var s in split)
								{
									DamageType type;
									if (Enum.TryParse(s.ToUpper(), out type))
									{
										creatureStats.damageResistances.Add(type);
									}
								}
							}
							if (line.StartsWith("Damage Immunities"))
							{
								creatureStats.damageImmunities = new List<DamageType>();
								var preSplit = line.Replace("Damage Immunities ", "").Replace("and", "");
								if (preSplit.Contains("nonmagical"))
								{
									creatureStats.damageImmunities.Add(DamageType.NONMAGICAL);
									preSplit = preSplit.Replace("bludgeoning", "").Replace("slashing", "").Replace("piercing", "");
								}
								var split = preSplit.Split(',');

								foreach (var s in split)
								{
									DamageType type;
									if (Enum.TryParse(s.ToUpper(), out type))
									{
										creatureStats.damageImmunities.Add(type);
									}
								}
							}
							if (line.StartsWith("Condition Immunities"))
							{
								var split = line.Replace("Condition Immunities ", "").Split(',');
								creatureStats.conditionImmunities = new List<ConditionType>();
								foreach (var s in split)
								{
									ConditionType type;
									if (Enum.TryParse(s.ToUpper(), out type))
									{
										creatureStats.conditionImmunities.Add(type);
									}
								}
							}

							if(line.StartsWith("Saving Throws"))
							{
								creatureStats.savingThrows = new PersonStats(SavingThrowHelper(line, "Str"), SavingThrowHelper(line, "Dex"), SavingThrowHelper(line, "Con"), SavingThrowHelper(line, "Int"), SavingThrowHelper(line, "Wis"), SavingThrowHelper(line, "Cha"), 0);
							}
						}

						if (abilitiesIndex != -1)
						{
							creatureStats.abilities = new List<string>();
							var endIndex = actionsIndex == -1 ? dataLines.Count : actionsIndex;

							for(int i = abilitiesIndex + 1; i < endIndex; i++)
							{
								if(dataLines[i] != string.Empty)
								{
									creatureStats.abilities.Add(dataLines[i]);
								}
							}
						}

						if (actionsIndex != -1)
						{
							creatureStats.actions = new List<string>();
							var endIndex = legendaryIndex == -1 ? dataLines.Count : legendaryIndex;

							for (int i = actionsIndex + 1; i < endIndex; i++)
							{
								if (dataLines[i] != string.Empty)
								{
									creatureStats.actions.Add(dataLines[i]);
								}
							}
						}

						if(legendaryIndex != -1)
						{
							creatureStats.legendaryActions = new List<string>();
							var endIndex = dataLines.Count;

							for (int i = legendaryIndex + 1; i < endIndex; i++)
							{
								if (dataLines[i] != string.Empty)
								{
									creatureStats.legendaryActions.Add(dataLines[i]);
								}
							}
						}
					}
				}
			}
			AssetDatabase.SaveAssets();
			Debug.Log("creatures built");
		}
	}
}