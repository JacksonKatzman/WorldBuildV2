using Game.Data.EventHandling.EventRecording;
using Game.Factions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Landmark : ITimeSensitive, IRecordable
{
    public string Name => name;
    public FactionSimulator faction;

    protected string name;
    public abstract void AdvanceTime();
}
