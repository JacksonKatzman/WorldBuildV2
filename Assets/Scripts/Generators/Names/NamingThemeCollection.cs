using Game.Data;
using Game.Enums;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Generators.Names
{
	[CreateAssetMenu(fileName = "ThemeCollection", menuName = "ScriptableObjects/Names/Theme Collection", order = 2)]
	public class NamingThemeCollection : SerializedScriptableObject
	{
		public List<WeightedString> nouns;
		public List<WeightedString> verbs;
		public List<WeightedString> adjectives;
		public List<WeightedString> townNouns;

		public List<WeightedString> consonants;
		public List<WeightedString> vowels;

		public List<TextAsset> maleNames;
		public List<TextAsset> femaleNames;

		public Dictionary<OrganizationType, TitleDictionary> titles;
		public List<string> titleQualifiers;

		[Button("Populate Consonants and Vowels")]
		private void PopulateConsonantsAndVowels()
		{
			if (vowels == null || vowels.Count == 0)
			{
				vowels = new List<WeightedString>(StaticNamingCollections.VOWELS);
			}
			if (consonants == null || consonants.Count == 0)
			{
				consonants = new List<WeightedString>(StaticNamingCollections.CONSONANTS);
			}
		}

		[TextArea]
		public string input;

		[Button("Add Inputs")]
		private void AddInputs()
		{
			var inputs = input.Split('\n');
			List<WeightedString> list = null;
			if(inputs[0].Contains("{N}"))
			{
				list = nouns;
			}
			else if(inputs[0].Contains("{V}"))
			{
				list = verbs;
			}
			else if(inputs[0].Contains("{A}"))
			{
				list = adjectives;
			}
			else if (inputs[0].Contains("{T}"))
			{
				list = townNouns;
			}

			if (list != null)
			{
				for(int i = 1; i < inputs.Length; i++)
				{
					var item = new WeightedString(inputs[i], 1, true, true);
					if (list.Where(x => x.value == inputs[i]).Count() == 0)
					{
						list.Add(item);
					}
				}
			}
		}

		public NamingThemeCollection() 
		{
			OutputLogger.LogWarning(">*> Default Constructor for ThemeCollection");
		}

		public NamingThemeCollection(NamingThemeCollection copy)
		{
			nouns = copy.nouns;
			verbs = copy.verbs;
			adjectives = copy.adjectives;
			townNouns = copy.townNouns;

			consonants = copy.consonants;
			vowels = copy.vowels;
			titles = copy.titles;
			titleQualifiers = copy.titleQualifiers;
		}
	}

	[Serializable, HideReferenceObjectPicker]
	public class TitlePair
	{
		public string maleTitle, femaleTitle;

		public TitlePair() { }
		public TitlePair(TitlePair other)
		{
			maleTitle = other.maleTitle;
			femaleTitle = other.femaleTitle;
		}

		public string GetTitle(Gender gender)
		{
			if(gender == Gender.MALE)
			{
				return maleTitle;
			}
			else if(gender == Gender.FEMALE)
			{
				return femaleTitle;
			}
			else
			{
				return SimRandom.RandomRange(0, 2) > 0 ? maleTitle : femaleTitle;
			}
		}
	}

	[Serializable]
	public class TitleDictionary : Dictionary<int, List<TitlePair>>
	{
		public void Merge(TitleDictionary other)
		{
			foreach (var pair in other)
			{
				if (this.ContainsKey(pair.Key))
				{
					this[pair.Key] = this[pair.Key].Union(pair.Value).ToList();
				}
				else
				{
					this.Add(pair.Key, pair.Value);
				}
			}
		}
	}

	public class MergableDictionary<T,V> : Dictionary<T,V>
	{
		public void Merge(MergableDictionary<T,V> other)
		{
			foreach (var pair in other)
			{
				if (this.ContainsKey(pair.Key))
				{
					this[pair.Key] = this[pair.Key].Union(pair.Value).ToList();
				}
				else
				{
					this.Add(pair.Key, pair.Value);
				}
			}
		}
	}
}
