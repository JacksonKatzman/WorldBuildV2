using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NameContainer
{
    public class WeightedNameContainer
	{
        public Dictionary<int, List<string>> weightedValues;
        public List<string> rawValues;

		public WeightedNameContainer(Dictionary<int, List<string>> weightedValues, List<string> rawValues)
		{
			this.weightedValues = weightedValues;
			this.rawValues = rawValues;
		}
	}

    public NamingRules rules;
    public List<string> MaleNames;
    public List<string> FemaleNames;
    public List<string> StaticMaleNames;
    public List<string> StaticFemaleNames;
    public WeightedNameContainer Vowels;
    public WeightedNameContainer StartConsonants;
    public WeightedNameContainer EndConsonants;

    public List<string> MaterialNames;
    public WeightedNameContainer MaterialSuffixes;

    public NameContainer(NameFormat format)
	{
        rules = format.rules;
        MaleNames = FormatTextAssetToList(format.maleNames);
        FemaleNames = FormatTextAssetToList(format.femaleNames);
        StaticMaleNames = FormatTextAssetToList(format.staticMaleNames);
        StaticFemaleNames = FormatTextAssetToList(format.staticFemaleNames);
        Vowels = FormatTextAssetsToDictionary(new TextAsset[] { format.vowels });
        StartConsonants = FormatTextAssetsToDictionary(new TextAsset[] { format.consonants, format.startPairs });
        EndConsonants = FormatTextAssetsToDictionary(new TextAsset[] { format.consonants, format.endPairs });

        MaterialNames = FormatTextAssetToList(format.materialNames);
        MaterialSuffixes = FormatTextAssetsToDictionary(new TextAsset[] { format.materialSuffixes });
	}

    private List<string> FormatTextAssetToList(TextAsset text)
	{
		string[] names = text.text.Split('\n');

        List<string> formattedList = new List<string>();

        foreach(string name in names)
		{
            formattedList.Add(name);
		}
        return formattedList;
	}

    private WeightedNameContainer FormatTextAssetsToDictionary(TextAsset[] texts)
    {
        Dictionary<int, List<string>> formattedDictionary = new Dictionary<int, List<string>>();
        List<string> rawList = new List<string>();

        foreach (TextAsset text in texts)
        {
            string[] names = text.text.Split('\n');

            foreach (string name in names)
            {
                string[] splitValues = name.Split(',');
                int key = Int32.Parse(splitValues[1]);
                string value = splitValues[0];
                if (!formattedDictionary.ContainsKey(key))
                {
                    formattedDictionary.Add(key, new List<string>());
                }
                formattedDictionary[key].Add(value);
                rawList.Add(value);
            }
        }
        return new WeightedNameContainer(formattedDictionary, rawList);
    }
}
