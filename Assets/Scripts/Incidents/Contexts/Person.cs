using Game.Enums;
using Game.Generators.Items;
using Game.Incidents;
using Game.Simulation;
using System;
using System.Collections.Generic;
using UnityEngine;

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
			Race = race == null ? new Race() : race;
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
		public int ID { get; set; }

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

		private Action OnDeathAction;
		public void DeployContext()
		{
			IncidentService.Instance.PerformIncidents(this);
			CheckForNaturalDeath();
		}

		public void UpdateContext()
		{
			NumIncidents = 1;
		}

		public void SetOnDeathAction(Action action)
		{
			OnDeathAction = action;
		}

		public void Die()
		{
			SimulationManager.Instance.world.RemoveContext(this);
			OnDeathAction?.Invoke();
		}

		private void CheckForNaturalDeath()
		{
			var cuspA = Race.UpperAgeLimit * 0.3f;
			var cuspB = Race.UpperAgeLimit * 0.85f;
			var deathChance = -Mathf.Atan(((cuspA + (cuspB - cuspA)) - Age) / (Mathf.Sqrt(cuspB - cuspA) * Mathf.PI / 2.0f)) / Mathf.PI + 0.5f;
			/*
			if(Age <= cuspA)
			{
				deathChance = Mathf.Pow(Age / cuspA, 2) / 7.0f;
			}
			else if(Age > cuspA && Age <= cuspB)
			{
				deathChance = ((Age - cuspA) / ((cuspB - cuspA) / 4) - Mathf.Atan((cuspB - cuspA) / 2 - (Age - cuspA))) / 10 + 0.3f;
			}
			else
			{
				deathChance = Mathf.Sqrt(0.731025f * ((Age - cuspB) / Race.UpperAgeLimit * 0.75f) + 1);
			}
			*/
			var randomValue = SimRandom.RandomFloat01();
			if(randomValue <= deathChance)
			{
				OutputLogger.Log(ID + " dying of natural causes.");
				Die();
			}			
		}
	}
}