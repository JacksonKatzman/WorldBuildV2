using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;

public class Person : ITimeSensitive
{
	public string name;
	public int age;
	public Gender gender;

	public List<Person> children;

	//Stat Block
	public PersonStats stats;

	//Priorities Block
	public Priorities priorities;

	public Person() : this(SimRandom.RandomRange(16, 69), (Gender)SimRandom.RandomRange(0, 2))
	{
	}

	public Person(int age, Gender gender)
	{
		this.age = age;
		this.gender = gender;

		name = NameGenerator.GeneratePersonFullName(gender);
	}

	public Person(int age, Gender gender, Person progenitor)
	{
		this.age = age;
		this.gender = gender;

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

	}
}
