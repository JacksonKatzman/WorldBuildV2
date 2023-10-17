using Game.Data;
using Game.Enums;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace Game.GUI.Popups
{
	public class MonsterInfoCardPopup : TypedPopup<MonsterInfoCardPopupConfig>
	{
		public TMP_Text monsterNameText;
		public TMP_Text monsterSizeAlignmentText;
		public TMP_Text acText;
		public TMP_Text hpText;
		public TMP_Text spdText;
		public TMP_Text climbText;
		public TMP_Text swimText;
		public TMP_Text flyText;
		public TMP_Text strengthText;
		public TMP_Text dexterityText;
		public TMP_Text constitutionText;
		public TMP_Text intelligenceText;
		public TMP_Text wisdomText;
		public TMP_Text charismaText;
		public TMP_Text savingThrowsText;
		public TMP_Text sensesText;
		public TMP_Text skillsText;
		public TMP_Text languagesText;
		public TMP_Text abilitiesText;
		public TMP_Text actionsText;
		public TMP_Text legendaryActionsText;

		public GameObject vulnerabilityDivider;
		public Dictionary<DamageType, GameObject> damageVulnerabilityIcons;
		public Dictionary<DamageType, GameObject> damageResistanceIcons;
		public Dictionary<DamageType, GameObject> damageImmunityIcons;
		public Dictionary<ConditionType, GameObject> conditionImmunityIcons;

		public List<GameObject> vulnerabilityObjects;
		public List<GameObject> resistancesObjects;
		public List<GameObject> immunitiesObjects;
		public List<GameObject> conditionImmunitiesObjects;
		public List<GameObject> abilitiesObjects;
		public List<GameObject> actionsObjects;
		public List<GameObject> legendaryActionsObjects;


		protected override bool CompareTo(MonsterInfoCardPopupConfig config)
		{
			return this.config.MonsterData == config.MonsterData;
		}

		protected override void Setup()
		{
			var data = config.MonsterData;

			monsterNameText.text = data.name;
			var isLegendary = data.legendary == true ? "Legendary" : string.Empty;
			monsterSizeAlignmentText.text = $"{isLegendary} {data.size.ToString()} {data.type.ToString()}, {data.alignment.ToString()}";
			acText.text = data.armorValue.ToString();
			hpText.text = data.health.ToString();

			spdText.text = $"{data.speed.ToString()} ft.";
			var canClimb = data.climbSpeed > 0 ? data.climbSpeed.ToString() : "N/A";
			var canSwim = data.swimSpeed > 0 ? data.swimSpeed.ToString() : "N/A";
			var canFly = data.flySpeed > 0 ? data.flySpeed.ToString() : "N/A";

			climbText.text = canClimb;
			swimText.text = canSwim;
			flyText.text = canFly;

			strengthText.text = BuildFullStatText(data.stats.strength);
			dexterityText.text = BuildFullStatText(data.stats.dexterity);
			constitutionText.text = BuildFullStatText(data.stats.constitution);
			intelligenceText.text = BuildFullStatText(data.stats.intelligence);
			wisdomText.text = BuildFullStatText(data.stats.wisdom);
			charismaText.text = BuildFullStatText(data.stats.charisma);

			var strSavingThrow = BuildInlineStatText("Str", data.savingThrows.strength, true);
			var dexSavingThrow = BuildInlineStatText("Dex", data.savingThrows.dexterity, true);
			var conSavingThrow = BuildInlineStatText("Con", data.savingThrows.constitution, true);
			var intSavingThrow = BuildInlineStatText("Int", data.savingThrows.intelligence, true);
			var wisSavingThrow = BuildInlineStatText("Wis", data.savingThrows.wisdom, true);
			var chaSavingThrow = BuildInlineStatText("Cha", data.savingThrows.charisma, false);
			var totalText = $"{strSavingThrow}{dexSavingThrow}{conSavingThrow}{intSavingThrow}{wisSavingThrow}{chaSavingThrow}";

			if (totalText == string.Empty)
			{
				savingThrowsText.gameObject.SetActive(false);
			}
			else
			{
				savingThrowsText.text = "Saving Throws: ";
				savingThrowsText.gameObject.SetActive(true);
				savingThrowsText.text += totalText;
			}

			sensesText.text = "Senses: ";
			foreach (var sense in data.senses)
			{
				sensesText.text += $"{sense.senseType} {sense.distance} ft., ";
			}

			sensesText.text += $"Passive Perception: {data.passivePerception}";

			skillsText.text = "Skills: " + BuildStringFromList(data.skills);
			languagesText.text = "Languages: " + BuildStringFromList(data.languages);

			abilitiesText.text = string.Empty;
			foreach (var ability in data.abilities)
			{
				abilitiesText.text += $"<i><b>{ability.abilityName}.</b></i> {ability.abilityDescription} \n";
			}

			ToggleListOfGameObjects(abilitiesObjects, abilitiesText.text != string.Empty);

			actionsText.text = string.Empty;
			foreach (var creatureAction in data.actions)
			{
				actionsText.text += $"<i><b>{creatureAction.actionName}.</b></i> {creatureAction.actionDescription} \n";
			}

			var weaponAttackRegex = new Regex(@"((Melee|Ranged) Weapon Attack)");
			actionsText.text = weaponAttackRegex.Replace(actionsText.text, m => string.Format("<i>{0}</i>", m.Groups[1].Value));

			ToggleListOfGameObjects(actionsObjects, actionsText.text != string.Empty);

			legendaryActionsText.text = string.Empty;
			foreach (var creatureAction in data.legendaryActions)
			{
				legendaryActionsText.text += $"<i><b>{creatureAction.actionName}.</b></i> {creatureAction.actionDescription} \n";
			}

			legendaryActionsText.text = weaponAttackRegex.Replace(legendaryActionsText.text, m => string.Format("<i>{0}</i>", m.Groups[1].Value));

			ToggleListOfGameObjects(legendaryActionsObjects, legendaryActionsText.text != string.Empty);

			//also need to make something to handle showing/hiding the immunities etc
			UpdateDamageIcons(damageVulnerabilityIcons, data.damageVulnerabilities, vulnerabilityObjects);
			UpdateDamageIcons(damageResistanceIcons, data.damageResistances, resistancesObjects);
			UpdateDamageIcons(damageImmunityIcons, data.damageImmunities, immunitiesObjects);

			foreach (var pair in conditionImmunityIcons)
			{
				pair.Value.SetActive(false);
			}
			if (data.conditionImmunities.Count > 0)
			{
				ToggleListOfGameObjects(conditionImmunitiesObjects, true);
				foreach (var immunity in data.conditionImmunities)
				{
					conditionImmunityIcons[immunity].SetActive(true);
				}
			}
			else
			{
				ToggleListOfGameObjects(conditionImmunitiesObjects, false);
			}

			if (data.damageVulnerabilities.Count == 0 && data.damageResistances.Count == 0
				&& data.damageImmunities.Count == 0 && data.conditionImmunities.Count == 0)
			{
				vulnerabilityDivider.SetActive(false);
			}
			else
			{
				vulnerabilityDivider.SetActive(true);
			}
		}

		private void UpdateDamageIcons(Dictionary<DamageType, GameObject> icons, List<DamageType> types, List<GameObject> associatedObjects)
		{
			foreach (var pair in icons)
			{
				pair.Value.SetActive(false);
			}
			if (types.Count > 0)
			{
				ToggleListOfGameObjects(vulnerabilityObjects, true);
				foreach (var vulnerability in types)
				{
					icons[vulnerability].SetActive(true);
				}
			}
			else
			{
				ToggleListOfGameObjects(associatedObjects, false);
			}
		}

		private string BuildStringFromList(List<string> stringList)
		{
			var text = string.Empty;
			for (int i = 0; i < stringList.Count; i++)
			{
				text += stringList[i];
				if (i != stringList.Count - 1)
				{
					text += ", ";
				}
			}

			return text;
		}

		private string BuildFullStatText(int stat)
		{
			return $"{stat} {BuildModifierText(CalculateModifier(stat))}";
		}

		private string BuildInlineStatText(string statName, int stat, bool addComma)
		{
			if (stat == 0)
			{
				return string.Empty;
			}
			var text = $"{statName}: {BuildModifierText(stat, true)}";
			var hasComma = (addComma && text != string.Empty) ? ", " : string.Empty;
			return $"{text}{hasComma}";
		}

		private string BuildModifierText(int modifier, bool optional = false)
		{
			if (optional && modifier == 0)
			{
				return string.Empty;
			}
			var symbol = modifier >= 0 ? "+" : "-";
			return $"({symbol}{Mathf.Abs(modifier)})";
		}
		private int CalculateModifier(int input)
		{
			return (input - 10) / 2;
		}
	}
}
