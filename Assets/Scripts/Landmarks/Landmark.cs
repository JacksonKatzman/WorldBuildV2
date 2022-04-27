using Game.Data.EventHandling.EventRecording;
using Game.Factions;
using Game.Generators.Items;
using Game.Incidents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ILandmark : ITimeSensitive, IRecordable, IAgeSensitive, IInventoryContainer
{
    public Faction Faction { get; set; }
}

public abstract class Landmark : ILandmark
{
    protected string name;
	protected int age;
    protected Faction faction;
	protected List<Item> inventory;

    public string Name => name;
	public int Age => age;
    public Faction Faction
	{
		get { return faction; }
		set { faction = value; }
	}

	public List<Item> Inventory => inventory;

	public virtual void AdvanceTime()
	{
		age++;
	}
}
