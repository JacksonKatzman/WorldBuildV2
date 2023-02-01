using Game.Enums;
using Game.Generators.Items;
using Game.Generators.Names;
using Game.Incidents;
using Game.Simulation;
using Game.Utilities;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	public class Person : IncidentContext, IPerson, IFactionAffiliated, IInventoryAffiliated, IAlignmentAffiliated
	{
		public Person() { }
		public Person(int age, Gender gender, Race race, Faction faction, int politicalPriority, int economicPriority,
			int religiousPriority, int militaryPriority, int influence, int wealth, int strength, int dexterity,
			int constitution, int intelligence, int wisdom, int charisma, bool worldPlayer, List<IPerson> parents = null, Inventory inventory = null)
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
			WorldPlayer = worldPlayer;
			Spouses = new List<IPerson>();
			Inventory = inventory == null ? new Inventory() : inventory;
			Parents = parents == null ? new List<IPerson>() : parents;
			Siblings = new List<IPerson>();

			if(Parents.Count > 0)
			{
				PersonName = AffiliatedFaction?.namingTheme.GenerateName(Gender, parents);
			}
			else
			{
				PersonName = AffiliatedFaction?.namingTheme.GenerateName(Gender);
			}
		}

		public Person(Gender gender, Race race, Faction faction, bool worldPlayer) :
			this(SimRandom.RandomRange(18, 55), gender, race, faction,
				SimRandom.RandomRange(0,7), SimRandom.RandomRange(0, 7),
				SimRandom.RandomRange(0, 7), SimRandom.RandomRange(0, 7),
				0, 0, SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				worldPlayer){ }

		public Person(Person parent, Gender gender = Gender.ANY)
		{

		}

		public override string Name => PersonName.GetTitledFullName(this);
		public CreatureName PersonName { get; set; }
		public int Age { get; set; }
		public Gender Gender { get; set; }
		public Race Race { get; set; }
		public Faction AffiliatedFaction { get; set; }
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
		public List<IPerson> Parents { get; set; }
		public List<IPerson> Spouses { get; set; }
		public List<IPerson> Siblings { get; set; }
		public List<IPerson> Children { get; set; }

		public int LawfulChaoticAlignmentAxis { get; set; }
		public int GoodEvilAlignmentAxis { get; set; }

		public bool WorldPlayer { get; set; }
		public bool Possessed { get; set; }

		private Action OnDeathAction;
		override public void DeployContext()
		{
			if (NumIncidents > 0)
			{
				IncidentService.Instance.PerformIncidents(this);
			}

			if(this.CheckDestroyed())
			{
				Die();
			}
		}

		override public void UpdateContext()
		{
			Age += 1;
			NumIncidents = WorldPlayer ? 1 : 0;
		}

		public Person CreateChild(bool majorPlayer)
		{
			var childAge = SimRandom.RandomRange(14, 35);
			var parents = new List<IPerson>() { this };
			if(Spouses.Count > 0)
			{
				parents.Add(SimRandom.RandomEntryFromList(Spouses));
			}
			var child = new Person(childAge, Enums.Gender.ANY, Race, AffiliatedFaction, 5, 5, 5, 5, 0, 0, 10, 10, 10, 10, 10, 10, majorPlayer, parents);

			return child;
		}

		public void GenerateFamily(bool generateParents)
		{
			if(generateParents && Parents.Count == 0)
			{
				Parents.Add(new Person(Gender.FEMALE, Race, null, false));
				Parents.Add(new Person(Gender.MALE, Race, null, false));
			}
		}

		override public void Die()
		{
			EventManager.Instance.Dispatch(new RemoveContextEvent(this));
			if (WorldPlayer)
			{
				IncidentService.Instance.ReportStaticIncident("{0} dies.", new List<IIncidentContext>() { this });
			}
		}
	}
}