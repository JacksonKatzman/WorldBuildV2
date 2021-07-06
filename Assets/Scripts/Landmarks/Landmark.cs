using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Landmark : ITimeSensitive
{
    public abstract void AdvanceTime();
}
