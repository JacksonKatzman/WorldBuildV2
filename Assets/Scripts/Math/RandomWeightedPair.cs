namespace Game.Factions
{
	public class RandomWeightedPair
	{
		public int weight;
		public int inverse;

		public RandomWeightedPair(int min, int max)
		{
			int randomValue = SimRandom.RandomRange(min, max);
			this.weight = randomValue;
			this.inverse = max - randomValue;
		}
	}
}