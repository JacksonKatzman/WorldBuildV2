using Game.Utilities;
using System.Collections.Generic;
using System.Linq;

namespace Game.Generators.Names
{
	public class ModifiableWeightedCollection
	{
		public List<WeightedString> weightedStrings;
		public Dictionary<int, List<string>> dictionary;

		public ModifiableWeightedCollection()
		{
			weightedStrings = new List<WeightedString>();
			dictionary = new Dictionary<int, List<string>>();
		}

		public ModifiableWeightedCollection(ModifiableWeightedCollection other)
		{
			weightedStrings = new List<WeightedString>(other.weightedStrings);
			dictionary = new Dictionary<int, List<string>>(other.dictionary);
		}

		public void AddWeightedStrings(List<WeightedString> strings)
		{
			//weightedStrings.AddRange(strings);
			foreach(var s in strings)
			{
				var overlap = weightedStrings.Where(x => x.value == s.value).ToList();
				if(overlap.Count > 0)
				{
					var item = overlap.First();
					item.weight += s.weight;
					if(item.weight < 0)
					{
						item.weight = 0;
					}
				}
				else
				{
					weightedStrings.Add(new WeightedString(s.value, s.weight));
				}
			}

			RebuildDictionary();
		}

		public void RandomizeWeights(int min, int max)
		{
			foreach(var w in weightedStrings)
			{
				w.weight = SimRandom.RandomRange(min, max + 1);
			}

			RebuildDictionary();
		}

		private void RebuildDictionary()
		{
			dictionary.Clear();
			foreach(var s in weightedStrings)
			{
				if(dictionary.ContainsKey(s.weight))
				{
					dictionary[s.weight].Add(s.value);
				}
				else
				{
					dictionary.Add(s.weight, new List<string>() { s.value });
				}
			}
		}
	}
}
