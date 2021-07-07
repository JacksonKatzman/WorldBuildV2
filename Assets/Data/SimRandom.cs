using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SimRandom
{
    private static System.Random rand => WorldHandler.Instance.seededRandom;

    public static int RandomInteger()
    {
        return rand.Next(-100000, 100000);
    }

    public static float RandomFloat01()
    {
        float percision = 100000.0f;
        return rand.Next(0, (int)percision) / percision;
    }

    public static int RandomRange(int minValue, int maxValue)
    {
        return rand.Next(minValue, maxValue);
    }
}
