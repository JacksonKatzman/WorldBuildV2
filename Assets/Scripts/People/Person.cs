using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;
using Game.Factions;
using Game.Data.EventHandling.EventRecording;
using Game.Generators.Items;

public class Person : ITimeSensitive, IRecordable
{
	public static int STARTING_PRIORITY_POINTS = 5;

	public string Name => GetName();

	private string name;
	public string personalTitle = "{0}";
	public int age;
	public Gender gender;
	public FactionSimulator faction;
	public LeadershipStructureNode governmentOffice;
	public int influence;

	public List<Person> children;

	public PersonStats stats;

	public Priorities priorities;

	public List<Item> inventory;

	private int naturalDeathAge;

	public Person() : this(SimRandom.RandomRange(16, 69), (Gender)SimRandom.RandomRange(0, 2), 0)
	{
	}

	public Person(int age, Gender gender, int startingInfluence, LeadershipStructureNode office = null)
	{
		this.age = age;
		this.gender = gender;
		this.influence = startingInfluence;
		this.governmentOffice = office;

		if (name == null)
		{
			name = NameGenerator.GeneratePersonFullName(gender);
		}
		if (stats == null)
		{
			GenerateStats();
		}
		if(priorities == null)
		{
			GeneratePriorities();
		}

		DetermineNaturalDeathAge();

		inventory = new List<Item>();

		OutputLogger.LogFormat("{0} was spawned at age {1}.", LogSource.PEOPLE, Name, age);
	}

	public Person(int age, Gender gender, Person progenitor) : this(age, gender, progenitor.influence/2)
	{
		var progenitorName = progenitor.Name.Split(' ');
		var progenitorSurname = progenitorName[progenitorName.Length - 1];
		name = NameGenerator.GeneratePersonFirstName(gender) + progenitorSurname;
	}

	public Person(int age, Gender gender, PersonStats stats) : this(age, gender, 0)
	{
		this.stats = stats;
	}

	public Person(int age, Gender gender, Priorities priorities) : this(age, gender, 0)
	{
		this.priorities = priorities;
	}

	public Person(string name, int age, Gender gender, List<Person> children, PersonStats stats, Priorities priorities)
	{
		this.name = name;
		this.age = age;
		this.gender = gender;
		this.children = children;
		this.stats = stats;
		this.priorities = priorities;
	}

	public void AdvanceTime()
	{
		TakeActions();

		age++;
		if(age >= naturalDeathAge)
		{
			//TakeActions();
			PersonGenerator.HandleDeath(this, "Natural Causes");
		}
	}

	private void TakeActions()
	{
		//ADD CHANCE FOR DEFINING EVENT

		SimAIManager.Instance.CallPersonAction(this, (governmentOffice != null));
	}

	private void GenerateStats()
	{
		stats = new PersonStats(SimRandom.RollXDY(4, 6, 1), SimRandom.RollXDY(4, 6, 1),
								SimRandom.RollXDY(4, 6, 1), SimRandom.RollXDY(4, 6, 1),
								SimRandom.RollXDY(4, 6, 1), SimRandom.RollXDY(4, 6, 1),
								SimRandom.RollXDY(4, 6, 1));
	}

	private void GeneratePriorities()
	{
		int[] pointAllocations = new int[5];
		for(int index = 0; index < STARTING_PRIORITY_POINTS; index++)
		{
			var randomIndex = SimRandom.RandomRange(0, pointAllocations.Length);
			pointAllocations[randomIndex]++;
		}

		priorities = new Priorities(pointAllocations[0], pointAllocations[1], pointAllocations[2], pointAllocations[3], pointAllocations[4]);
	}

	private void DetermineNaturalDeathAge()
	{
		var fullCycle = (int)(SimRandom.RandomRange(4,8) * stats.constitution);
		var modifiedCycle = (int)(age + (SimRandom.RandomRange(2,6) * stats.constitution));

		naturalDeathAge = Mathf.Max(fullCycle, modifiedCycle);
	}

	private string GetName()
	{
		string withPersonal = string.Format(personalTitle, name);
		string withOffice = (governmentOffice != null ? governmentOffice.title : "{0}");
		return string.Format(withOffice, withPersonal);
	}
}
