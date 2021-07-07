using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;

public class Person : ITimeSensitive
{
	public static int STARTING_PRIORITY_POINTS = 5;

	public string name;
	public int age;
	public Gender gender;

	public List<Person> children;

	//Stat Block
	public PersonStats stats;

	//Priorities Block
	public Priorities priorities;

	private int naturalDeathAge;

	public Person() : this(SimRandom.RandomRange(16, 69), (Gender)SimRandom.RandomRange(0, 2))
	{
	}

	public Person(int age, Gender gender)
	{
		this.age = age;
		this.gender = gender;

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
	}

	public Person(int age, Gender gender, Person progenitor) : this(age, gender)
	{
		var progenitorName = progenitor.name.Split(' ');
		var progenitorSurname = progenitorName[progenitorName.Length - 1];
		name = NameGenerator.GeneratePersonFirstName(gender) + progenitorSurname;
	}

	public Person(int age, Gender gender, PersonStats stats) : this(age, gender)
	{
		this.stats = stats;
	}

	public Person(int age, Gender gender, Priorities priorities) : this(age, gender)
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
		var fullCycle = (int)(((5 * SimRandom.RandomFloat01()) + 1) * stats.constitution);
		var modifiedCycle = (int)(age + ((SimRandom.RandomFloat01() * stats.constitution) + 1));

		naturalDeathAge = Mathf.Max(fullCycle, modifiedCycle);
	}
}
