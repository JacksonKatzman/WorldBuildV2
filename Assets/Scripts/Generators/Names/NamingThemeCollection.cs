using Game.Data;
using Game.Debug;
using Game.Enums;
using Game.Utilities;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Generators.Names
{
	[CreateAssetMenu(fileName = "ThemeCollection", menuName = "ScriptableObjects/Names/Theme Collection", order = 2)]
	public class NamingThemeCollection : AbstractNamingThemeObject
	{
		public List<string> nouns;
		public List<string> verbs;
		public List<string> adjectives;
		public List<string> placeNouns;

		/*
		[SerializeField, ValueDropdown("GetListOptions", IsUniqueList = true)]
		private string listToAddTo;

		[TextArea]
		public string input;
		*/

		public NamingThemeCollection() 
		{
			GetListCollection();
		}

		public NamingThemeCollection(NamingThemeCollection copy)
		{
			nouns = copy.nouns;
			verbs = copy.verbs;
			adjectives = copy.adjectives;
			placeNouns = copy.placeNouns;
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
	public class TitleDictionary : Dictionary<int, TitlePairList>
	{
		public void Merge(TitleDictionary other)
		{
			foreach (var pair in other)
			{
				if (this.ContainsKey(pair.Key))
				{
					var first = this[pair.Key].titlePairs;
					var second = other[pair.Key].titlePairs;
					var combined = first.Union(second);
					this[pair.Key] = new TitlePairList(combined.ToList());
				}
				else
				{
					this.Add(pair.Key, pair.Value);
				}
			}
		}
	}

	[HideReferenceObjectPicker]
	public class TitlePairList
	{
		[ListDrawerSettings(CustomAddFunction = "AddTitlePair")]
		public List<TitlePair> titlePairs;
		public TitlePairList()
		{
			titlePairs = new List<TitlePair>();
		}

		public TitlePairList(List<TitlePair> pairs)
		{
			titlePairs = new List<TitlePair>(pairs);
		}
		private void AddTitlePair()
		{
			titlePairs.Add(new TitlePair());
		}
	}
}
