using Game.Enums;
using Game.Generators.Items;
using Game.Incidents;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class Person : IIncidentContext, IFactionAffiliated
	{
		public Person() { }
		public Person(int age, Gender gender, Race race, Faction faction, int politicalPriority, int economicPriority,
			int religiousPriority, int militaryPriority, int influence, int wealth, int strength, int dexterity,
			int constitution, int intelligence, int wisdom, int charisma, List<Item> inventory = null, List<Person> parents = null)
		{
			Age = age;
			Race = race;
			Gender = gender == Gender.ANY ? (Gender)(SimRandom.RandomRange(0, 2)) : gender;
			AffiliatedFaction = faction;
			PoliticalPriority = politicalPriority;
			EconomicPriority = economicPriority;
			ReligiousPriority = religiousPriority;
			MilitaryPriority = militaryPriority;
			Influence = influence;
			Wealth = wealth;
			Strength = strength;
			Dexterity = dexterity;
			Constitution = constitution;
			Intelligence = intelligence;
			Wisdom = wisdom;
			Charisma = charisma;
			Inventory = inventory == null ? new List<Item>() : inventory;
			Parents = parents == null ? new List<Person>() : parents;
		}

		public Type ContextType => typeof(Person);

		public int NumIncidents { get; set; }

		public int ParentID => -1;
		public int Age { get; set; }
		public Gender Gender { get; set; }
		public Race Race { get; set; }
		public Faction AffiliatedFaction { get; private set; }
		public int PoliticalPriority { get; set; }
		public int EconomicPriority { get; set; }
		public int ReligiousPriority { get; set; }
		public int MilitaryPriority { get; set; }
		public int Influence { get; set; }
		public int Wealth { get; set; }
		public int Strength { get; set; }
		public int Dexterity { get; set; }
		public int Constitution { get; set; }
		public int Intelligence { get; set; }
		public int Wisdom { get; set; }
		public int Charisma { get; set; }
		public List<Item> Inventory { get; set; }
		public List<Person> Parents { get; set; }
		public List<Person> Spouses { get; set; }
		public List<Person> Children { get; set; }
		public void DeployContext()
		{
			
		}

		public void UpdateContext()
		{
			
		}
	}
}