using Game.Enums;
using Game.Generators.Items;
using Game.Incidents;
using Game.Simulation;
using Game.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	public class Person : IncidentContext, IFactionAffiliated, IInventoryAffiliated, IAlignmentAffiliated
	{
		public Person() { }
		public Person(int age, Gender gender, Race race, Faction faction, int politicalPriority, int economicPriority,
			int religiousPriority, int militaryPriority, int influence, int wealth, int strength, int dexterity,
			int constitution, int intelligence, int wisdom, int charisma, List<Person> parents = null, Inventory inventory = null)
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
			Inventory = inventory == null ? new Inventory() : inventory;
			Parents = parents == null ? new List<Person>() : parents;

			if(Parents.Count > 0)
			{
				Name = AffiliatedFaction?.namingTheme.GenerateName(Gender, parents);
			}
			else
			{
				Name = AffiliatedFaction?.namingTheme.GenerateName(Gender);
			}
		}

		public override string Name { get => GetFullName(); set => name = value; }
		public int Age { get; set; }
		public Gender Gender { get; set; }
		public Race Race { get; set; }
		public Faction AffiliatedFaction { get; private set; }
		public OrganizationPosition OfficialPosition { get; set; }
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
		public Inventory Inventory { get; set; }
		public List<Person> Parents { get; set; }
		public List<Person> Spouses { get; set; }
		public List<Person> Children { get; set; }

		public int LawfulChaoticAlignmentAxis { get; set; }
		public int GoodEvilAlignmentAxis { get; set; }

		private string name;
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

		public Person CreateChild()
		{
			var childAge = SimRandom.RandomRange(14, 35);
			var child = new Person(childAge, Enums.Gender.ANY, Race, AffiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10, new List<Person>() { this });

			return child;
		}

		override public void Die()
		{
			EventManager.Instance.Dispatch(new RemoveContextEvent(this));
			IncidentService.Instance.ReportStaticIncident("{0} dies.", new List<IIncidentContext>() { this });
		}

		private void CheckDestroyed()
		{
			var cuspA = Race.MaxAge * 0.3f;
			var cuspB = Race.MaxAge * 0.85f;
			var deathChance = -Mathf.Atan(((cuspA + (cuspB - cuspA)) - Age) / (Mathf.Sqrt(cuspB - cuspA) * Mathf.PI / 2.0f)) / Mathf.PI + 0.5f;

			var randomValue = SimRandom.RandomFloat01();
			if(randomValue <= deathChance)
			{
				Die();
			}			
		}

		public string GetSurname()
		{
			var names = name.Split(' ');
			return names[names.Length - 1];
		}

		private string GetFullName()
		{
			var fullName = name;
			if(OfficialPosition != null)
			{
				fullName = string.Format(OfficialPosition.titlePair.GetTitle(Gender), fullName);
			}

			return fullName;
		}
	}
}