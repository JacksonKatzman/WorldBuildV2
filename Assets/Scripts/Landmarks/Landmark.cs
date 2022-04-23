using Game.Data.EventHandling.EventRecording;
using Game.Factions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILandmark : ITimeSensitive, IRecordable
{
    public Faction Faction { get; set; }
}

public abstract class Landmark : ILandmark
{
    protected string name;
    protected Faction faction;

    public string Name => name;
    public Faction Faction
	{
		get { return faction; }
		set { faction = value; }
	}

	public virtual void AdvanceTime()
	{
	}
}
