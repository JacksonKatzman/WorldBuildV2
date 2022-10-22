using System.Collections.Generic;

public static class SimRandom
{
    //private static System.Random rand => SimulationManager.Instance.seededRandom;
    public static int seed = 69;
    public static System.Random rand = new System.Random(seed);

    public static int RandomInteger()
    {
        return rand.Next(-100000, 100000);
    }

    public static float RandomFloat01()
    {
        float precision = 100000.0f;
        return rand.Next(0, (int)precision) / precision;
    }

    public static int RandomRange(int minValue, int maxValue)
    {
        return rand.Next(minValue, maxValue);
    }

    public static int RollXDY(int numDice, int maxRoll, int dropLowest = 0)
	{
        List<int> rolls = new List<int>();
        for(int index = 0; index < numDice; index++)
		{
            rolls.Add(rand.Next(1, maxRoll + 1));
		}

        rolls.Sort();

        int result = 0;
        for (int index = 0; index < numDice - dropLowest; index++)
        {
            result += rolls[index];
        }

        return result;
    }

    public static T RandomEntryFromList<T>(List<T> collection)
	{
        var randomIndex = RandomRange(0, collection.Count);
        return collection[randomIndex];
	}

    public static T RandomEntryFromWeightedDictionary<T>(Dictionary<int, List<T>> collection)
	{
        var totalWeight = 0;
        foreach(var pair in collection)
		{
            totalWeight += pair.Key;
		}

        T randomItem = default(T);
        var randomWeight = RandomRange(0, totalWeight);

        foreach (var pair in collection)
        {
            if((randomWeight -= pair.Key) <= 0)
			{
                return RandomEntryFromList(pair.Value);
			}
        }

        return randomItem;
    }
}
