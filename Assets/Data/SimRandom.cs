using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimRandom
{
    private static System.Random rand => SimulationManager.Instance.seededRandom;

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
}
