using Game.Enums;
using Game.Generators.Items;
using Game.Incidents;
using Game.Simulation;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	public class Person : IncidentContext, IFactionAffiliated, IInventoryAffiliated
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
		override public void DeployContext()
		{
			IncidentService.Instance.PerformIncidents(this);
			CheckDestroyed();
		}

		override public void UpdateContext()
		{
			Age += 1;
			NumIncidents = 1;
		}

		public void SetOnDeathAction(Action action)
		{
			OnDeathAction = action;
		}

		override public void Die()
		{
			SimulationManager.Instance.world.RemoveContext(this);
			OnDeathAction?.Invoke();
		}

		private void CheckDestroyed()
		{
			var cuspA = Race.UpperAgeLimit * 0.3f;
			var cuspB = Race.UpperAgeLimit * 0.85f;
			var deathChance = -Mathf.Atan(((cuspA + (cuspB - cuspA)) - Age) / (Mathf.Sqrt(cuspB - cuspA) * Mathf.PI / 2.0f)) / Mathf.PI + 0.5f;

			var randomValue = SimRandom.RandomFloat01();
			if(randomValue <= deathChance)
			{
				Die();
			}			
		}
	}
}