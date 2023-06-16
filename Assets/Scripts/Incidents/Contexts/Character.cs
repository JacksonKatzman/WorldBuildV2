using Game.Enums;
using Game.Generators.Items;
using Game.Generators.Names;
using Game.Incidents;
using Game.Simulation;
using Game.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public class Character : IncidentContext, ICharacter, IFactionAffiliated, IInventoryAffiliated, IAlignmentAffiliated
	{
		public Character() 
		{
			Parents = new List<Character>();
			Spouses = new List<Character>();
			Siblings = new List<Character>();
			Children = new List<Character>();
			//Race = new Race(new RacePreset());
			//CharacterName = new CharacterName("FORMAT");
			Inventory = new Inventory();
		}
		public Character(int age, Gender gender, Race race, Faction faction, int politicalPriority, int economicPriority,
			int religiousPriority, int militaryPriority, int influence, int wealth, int strength, int dexterity,
			int constitution, int intelligence, int wisdom, int charisma, bool majorCharacter, List<Character> parents = null, Inventory inventory = null)
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
			MajorCharacter = majorCharacter;
			Spouses = new List<Character>();
			Inventory = inventory == null ? new Inventory() : inventory;
			Parents = parents == null ? new List<Character>() : parents;
			Siblings = new List<Character>();
			Children = new List<Character>();

			if(Parents.Count > 0)
			{
				CharacterName = AffiliatedFaction?.namingTheme.GenerateName(Gender, parents);
			}
			else
			{
				CharacterName = AffiliatedFaction?.namingTheme.GenerateName(Gender);
			}
		}

		public Character(Gender gender, Race race, Faction faction, bool majorCharacter, List<Character> parents = null) :
			this(SimRandom.RandomRange(18, 55), gender, race, faction,
				SimRandom.RandomRange(0,7), SimRandom.RandomRange(0, 7),
				SimRandom.RandomRange(0, 7), SimRandom.RandomRange(0, 7),
				0, 0, SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				majorCharacter, parents){ }
		public Character(int age, Gender gender, Race race, Faction faction, bool majorCharacter, List<Character> parents = null) :
			this(age, gender, race, faction,
				SimRandom.RandomRange(0, 7), SimRandom.RandomRange(0, 7),
				SimRandom.RandomRange(0, 7), SimRandom.RandomRange(0, 7),
				0, 0, SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				majorCharacter, parents){ }

		public Character(Gender gender, Race race, Faction faction, int politicalPriority,
			int economicPriority, int religiousPriority, int militaryPriority, bool majorCharacter, List<Character> parents = null) : 
			this(SimRandom.RandomRange(18, 55), gender, race, faction,
				politicalPriority, economicPriority,
				religiousPriority, militaryPriority,
				0, 0, SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				majorCharacter, parents){ }

		public Character(int age, Gender gender, Race race, Faction faction, int politicalPriority,
			int economicPriority, int religiousPriority, int militaryPriority, bool majorCharacter, List<Character> parents = null) :
			this(age, gender, race, faction,
				politicalPriority, economicPriority,
				religiousPriority, militaryPriority,
				0, 0, SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				SimRandom.RandomRange(5, 14), SimRandom.RandomRange(5, 14),
				majorCharacter, parents){ }

		public Character(Character parent, Gender gender = Gender.ANY)
		{

		}

		public Character(Faction affiliatedFaction)
		{
			AffiliatedFaction = affiliatedFaction;
			Race = affiliatedFaction.MajorityRace;
			Age = SimRandom.RandomRange(Race.MinAge, Race.MaxAge);
			Gender = (Gender)(SimRandom.RandomRange(0, 2));
			CharacterName = affiliatedFaction.namingTheme.GenerateName(Gender);
			MajorCharacter = false;
			Parents = new List<Character>();
			Spouses = new List<Character>();
			Siblings = new List<Character>();
			Children = new List<Character>();
		}

		public override string Name => CharacterName.GetTitledFullName(this);
		public CharacterName CharacterName { get; set; }
		public int Age { get; set; }
		public Gender Gender { get; set; }
		public Race Race { get; set; }
		public Faction AffiliatedFaction { get; set; }
		public Organization Organization { get; set; }
		public OrganizationPosition OfficialPosition => GetOfficialPosition();
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
		public List<Character> Parents { get; set; }
		public List<Character> Spouses { get; set; }
		public List<Character> Siblings { get; set; }
		public List<Character> Children { get; set; }
		
		//public List<Character> Family => new List<Character>().Union(Parents).Union(Spouses).Union(Siblings).Union(Children).ToList();

		public int LawfulChaoticAlignmentAxis { get; set; }
		public int GoodEvilAlignmentAxis { get; set; }

		public bool MajorCharacter { get; set; }
		public bool Possessed { get; set; }

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
			NumIncidents = MajorCharacter ? 1 : 0;
		}

		public Character CreateChild(bool majorPlayer)
		{
			var childAge = SimRandom.RandomRange(14, 35);
			var parents = new List<Character>() { this };
			if(Spouses.Count > 0)
			{
				parents.Add(SimRandom.RandomEntryFromList(Spouses));
			}
			var child = new Character(childAge, Enums.Gender.ANY, Race, AffiliatedFaction, majorPlayer, parents);
			Children.Add(child);

			return child;
		}

		public void GenerateFamily(bool generateParents, bool canGenerateSpouse)
		{
			if(generateParents)
			{
				if(Parents.Count(x => x.Gender == Gender.MALE) < 1)
				{
					var father = new Character(Gender.MALE, Race, AffiliatedFaction, false);
					Parents.Add(father);
					ContextDictionaryProvider.AddContext(father);
				}
				if(Parents.Count(x => x.Gender == Gender.FEMALE) < 1)
				{
					var mother = new Character(Gender.FEMALE, Race, AffiliatedFaction, false);
					Parents.Add(mother);
					ContextDictionaryProvider.AddContext(mother);
				}
			}
			if(canGenerateSpouse && Spouses.Count == 0)
			{
				if(SimRandom.RandomBool())
				{
					var gender = Gender == Gender.MALE ? Gender.FEMALE : Gender.MALE;
					var spouse = new Character(gender, Race, AffiliatedFaction, false);
					Spouses.Add(spouse);
					ContextDictionaryProvider.AddContext(spouse);
				}
			}
			if (Siblings.Count == 0)
			{
				//Change to be a curve later
				var numSiblings = SimRandom.RandomRange(0, 5);
				for (int i = 0; i < numSiblings; i++)
				{
					var sibling = new Character(Gender.ANY, Race, AffiliatedFaction, false, Parents);
					Siblings.Add(sibling);
					ContextDictionaryProvider.AddContext(sibling);
				}
			}
		}

		override public void Die()
		{
			if(MajorCharacter)
			{
				IncidentService.Instance.ReportStaticIncident("{0} dies.", new List<IIncidentContext>() { this });
			}
			EventManager.Instance.Dispatch(new RemoveContextEvent(this));
		}

		public override void LoadContextProperties()
		{
			Race = SaveUtilities.ConvertIDToContext<Race>(contextIDLoadBuffers["Race"][0]);
			AffiliatedFaction = SaveUtilities.ConvertIDToContext<Faction>(contextIDLoadBuffers["AffiliatedFaction"][0]);
			Parents = SaveUtilities.ConvertIDsToContexts<Character>(contextIDLoadBuffers["Parents"]);
			Spouses = SaveUtilities.ConvertIDsToContexts<Character>(contextIDLoadBuffers["Spouses"]);
			Siblings = SaveUtilities.ConvertIDsToContexts<Character>(contextIDLoadBuffers["Siblings"]);
			Children = SaveUtilities.ConvertIDsToContexts<Character>(contextIDLoadBuffers["Children"]);
			Inventory?.LoadContextProperties();

			contextIDLoadBuffers.Clear();
		}

		private OrganizationPosition GetOfficialPosition()
		{
			if(Organization == null)
			{
				return null;
			}

			if(Organization.Contains(this, out var position))
			{
				return position;
			}
			else
			{
				return null;
			}
		}
	}
}