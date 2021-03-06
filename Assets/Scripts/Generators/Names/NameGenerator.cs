using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using UnityEngine;
using Game.Enums;
using Game.Races;

public class NameGenerator
{
	private static int SYLLABALIC_RATIO = 30;
	private static int MODIFIED_RATIO = 60;
	private static int STATIC_RATIO = 10;

	private static float FIRST_PHONEME_MORPH_PERCENTAGE = 0.4f;
	private static float VOWEL_PHONEME_MORPH_PERCENTAGE = 0.5f;
	private static float TRUNCATE_PERCENTAGE = 0.4f;

	//private static NameContainer defaultNameContainer => DataManager.Instance.PrimaryNameContainer;

	public static string GeneratePersonFirstName(Race race, Gender gender)
	{
		int combinedRatio = SYLLABALIC_RATIO + MODIFIED_RATIO;
		int randomWeight = SimRandom.RandomRange(0, combinedRatio);
		var container = DataManager.Instance.nameContainers[race];
		if((randomWeight -= SYLLABALIC_RATIO) < 0)
		{
			return SyllabalicNameGeneration(container, container.rules.FirstNameSyllables.x, container.rules.FirstNameSyllables.y);
		}
		else
		{
			return ModifiedNameGeneration(container, gender, container.rules.TweakFirstNames);
		}
	}
	public static string GeneratePersonSurname(Race race)
	{
		var container = DataManager.Instance.nameContainers[race];
		return SyllabalicNameGeneration(container, container.rules.LastNameSyllables.x, container.rules.LastNameSyllables.y);
	}

	public static string GeneratePersonFullName(Race race, Gender gender)
	{
		int combinedRatio = SYLLABALIC_RATIO + MODIFIED_RATIO + STATIC_RATIO;
		int randomWeight = SimRandom.RandomRange(0, combinedRatio);
		var container = DataManager.Instance.nameContainers[race];
		if ((randomWeight -= STATIC_RATIO) < 0)
		{
			return StaticNameGeneration(container, gender);
		}
		else
		{
			return GeneratePersonFirstName(race, gender) + " " + GeneratePersonSurname(race);
		}
	}

	public static string GenerateMaterialName()
	{
		//make better later
		/*
		var prefix = SyllabalicNameGeneration(defaultNameContainer, 2, 4);
		var suffix = GetWeightedSelectionFromDictionary(defaultNameContainer.MaterialSuffixes.weightedValues);
		return prefix + suffix;
		*/
		return "Material_" + SimRandom.RandomInteger();
	}

	public static string GenerateRelicName()
	{
		//make better later
		return "Relic_" + SimRandom.RandomInteger();
	}

	public static string GenerateHolidayName()
	{
		return "Holiday_" + SimRandom.RandomInteger();
	}

	public static void GenerateTitleStructure(List<LeadershipTier> leadershipStructure, Priorities priorities, Race race)
	{
		var numTiers = leadershipStructure.Count;
		var prioType = priorities.SortedList()[0];
		var container = DataManager.Instance.nameContainers[race];
		var titleDictionary = container.Titles[prioType].weightedValues;

		for (int i = 0; i < numTiers; i++)
		{
			var titlePoints = (numTiers * (numTiers - i)) - leadershipStructure[i].Count + 1;
			var difference = int.MaxValue;
			var closest = 0;

			foreach(var weight in titleDictionary.Keys)
			{
				var testDif = Mathf.Abs(weight - titlePoints);
				if (testDif < difference)
				{
					difference = testDif;
					closest = weight;
				}
			}

			var baseTitle = SimRandom.RandomEntryFromList(titleDictionary[closest]);

			titlePoints -= closest;

			string titleMod = "{0}";
			if (titlePoints > 0)
			{
				titleMod = SimRandom.RandomEntryFromList(container.TitleModifiers.rawValues);
			}

			var genderTitles = new Dictionary<Gender, string>();
			genderTitles.Add(Gender.MALE, string.Format(titleMod, baseTitle));
			genderTitles.Add(Gender.FEMALE, string.Format(titleMod, baseTitle));

			if (baseTitle.Contains("/"))
			{
				var divided = baseTitle.Split('/');
				genderTitles[Gender.MALE] = string.Format(titleMod, divided[0]);
				genderTitles[Gender.FEMALE] = string.Format(titleMod, divided[1]);
			}

			foreach(var node in leadershipStructure[i])
			{
				node.genderedTitles = genderTitles;
			}	
		}
	}

	private static string ModifiedNameGeneration(NameContainer container, Gender gender, bool tweak)
	{
		string currentName = string.Empty;
		List<string> possibleNames = new List<string>();
		if(gender == Gender.MALE)
		{
			possibleNames.AddRange(container.MaleNames);
		}
		else if(gender == Gender.FEMALE)
		{
			possibleNames.AddRange(container.FemaleNames);
		}
		else
		{
			possibleNames.AddRange(container.MaleNames);
			possibleNames.AddRange(container.FemaleNames);
		}

		int randomIndex = SimRandom.RandomRange(0, possibleNames.Count);
		currentName = possibleNames[randomIndex];

		if(tweak && currentName.Length > 2)
		{
			currentName = TweakName(container, currentName);
		}

		if (currentName.ToLower() == "null\r")
		{
			currentName = "nulloo\r";
		}

		return ToTitleCase(currentName);
	}

	private static string SyllabalicNameGeneration(NameContainer container, int minSyllables, int maxSyllables)
	{
		string currentName = string.Empty;
		bool vowel = (SimRandom.RandomFloat01() > 0.5f);
		int targetSyllables = SimRandom.RandomRange(minSyllables, maxSyllables + 1);
		List<string> syllables = new List<string>();

		for (int index = 0; index < targetSyllables; index++)
		{
			if (vowel)
			{
				syllables = GetWeightedSelectionFromDictionary(container.Vowels.weightedValues);
			}
			else
			{
				bool lastSyllable = (index == (targetSyllables - 1));
				if(lastSyllable)
				{
					syllables = GetWeightedSelectionFromDictionary(container.EndConsonants.weightedValues);
				}
				else
				{
					syllables = GetWeightedSelectionFromDictionary(container.StartConsonants.weightedValues);
				}
			}

			int randomIndex = SimRandom.RandomRange(0, syllables.Count);
			currentName += syllables[randomIndex];
			vowel = !vowel;
			syllables.Clear();
		}

		if (currentName.ToLower() == "null\r")
		{
			currentName = "nulloo\r";
		}

		return ToTitleCase(currentName);
	}

	private static string StaticNameGeneration(NameContainer container, Gender gender)
	{
		List<string> possibleNames = new List<string>();
		if (gender == Gender.MALE)
		{
			possibleNames.AddRange(container.StaticMaleNames);
		}
		else if (gender == Gender.FEMALE)
		{
			possibleNames.AddRange(container.StaticFemaleNames);
		}
		else
		{
			possibleNames.AddRange(container.StaticMaleNames);
			possibleNames.AddRange(container.StaticFemaleNames);
		}

		int randomIndex = SimRandom.RandomRange(0, possibleNames.Count);
		return possibleNames[randomIndex];
	}

	private static List<string> GetWeightedSelectionFromDictionary(Dictionary<int, List<string>> dictionary)
	{
		int ratioTotal = 0;
		foreach (int key in dictionary.Keys)
		{
			ratioTotal += key;
		}

		List<string> returnList = new List<string>();
		int randomWeight = SimRandom.RandomRange(0, ratioTotal);
		foreach (int key in dictionary.Keys)
		{
			if ((randomWeight -= key) < 0)
			{
				returnList.AddRange(dictionary[key]);
				break;
			}
		}
		return returnList;
	}

	private static string ToTitleCase(string name)
	{
		return name.Substring(0, 1).ToUpper() + name.Substring(1).ToLower();
	}

	private static string TweakName(NameContainer container, string name)
	{
		string currentName = name;

		AttemptFirstPhonemeMorph(container, ref currentName);

		AttemptTruncateName(container, ref currentName);

		AttemptTweakVowel(container, ref currentName);
		
		return currentName;
	}

	private static void AttemptFirstPhonemeMorph(NameContainer container, ref string currentName)
	{
		bool shouldMorph = (SimRandom.RandomFloat01() > FIRST_PHONEME_MORPH_PERCENTAGE);
		if(shouldMorph)
		{
			List<string> possibleReplacements = new List<string>();
			possibleReplacements = GetWeightedSelectionFromDictionary(container.StartConsonants.weightedValues);

			string start = currentName.Substring(0, 1).ToLower();
			if (container.Vowels.rawValues.Contains(start))
			{
				string newStart = possibleReplacements[SimRandom.RandomRange(0, possibleReplacements.Count)];
				currentName = newStart + currentName;
			}
			else if (container.StartConsonants.rawValues.Contains(start))
			{
				var replacement = possibleReplacements[SimRandom.RandomRange(0, possibleReplacements.Count)];
				ReplaceCharAtIndexWithString(ref currentName, 0, replacement);
			}
		}
	}

	private static void AttemptTruncateName(NameContainer container, ref string currentName)
	{
		bool shouldMorph = (SimRandom.RandomFloat01() > TRUNCATE_PERCENTAGE);
		if (shouldMorph && currentName.Length > 4)
		{
			currentName = currentName.Substring(0, currentName.Length - 2);
		}
		if (container.StartConsonants.rawValues.Contains(currentName[currentName.Length-1].ToString()))
		{
			var vowels = GetWeightedSelectionFromDictionary(container.Vowels.weightedValues);
			currentName += vowels[SimRandom.RandomRange(0, vowels.Count)];
		}
	}

	private static void AttemptTweakVowel(NameContainer container, ref string currentName)
	{
		bool shouldMorph = (SimRandom.RandomFloat01() > VOWEL_PHONEME_MORPH_PERCENTAGE);
		if (shouldMorph)
		{
			List<int> vowelLocations = new List<int>();
			for (int index = 0; index < currentName.Length; index++)
			{
				if (container.Vowels.rawValues.Contains(currentName[index].ToString()))
				{
					vowelLocations.Add(index);
				}
			}

			if (vowelLocations.Count > 0)
			{
				int randomIndex = vowelLocations[SimRandom.RandomRange(0, vowelLocations.Count)];
				var possibleReplacements = GetWeightedSelectionFromDictionary(container.Vowels.weightedValues);
				var replacement = possibleReplacements[SimRandom.RandomRange(0, possibleReplacements.Count)];
				ReplaceCharAtIndexWithString(ref currentName, randomIndex, replacement);
			}
		}
	}

	private static void ReplaceCharAtIndexWithString(ref string currentName, int index, string replacement)
	{
		var aStringBuilder = new StringBuilder(currentName);
		aStringBuilder.Remove(index, 1);
		aStringBuilder.Insert(index, replacement);
		currentName = aStringBuilder.ToString();
	}
}
