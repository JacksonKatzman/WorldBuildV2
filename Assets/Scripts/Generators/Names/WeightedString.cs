using Newtonsoft.Json;
using System;

namespace Game.Generators.Names
{
	[Serializable]
	public class WeightedString
	{
		public string value;
		public int weight;
		public bool allowedAtBeginning = true;
		public bool allowedAtEnd = true;

		public WeightedString(string value, int weight)
		{
			this.value = value;
			this.weight = weight;
		}

		[JsonConstructor]
		public WeightedString(string value, int weight, bool allowedAtBeginning, bool allowedAtEnd) : this(value, weight)
		{
			this.allowedAtBeginning = allowedAtBeginning;
			this.allowedAtEnd = allowedAtEnd;
		}
	}
}
