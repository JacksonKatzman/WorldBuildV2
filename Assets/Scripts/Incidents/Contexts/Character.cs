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
	public class Character : IncidentContext, ICharacter, IFactionAffiliated, IInventoryAffiliated, IAlignmentAffiliated, IRaceAffiliated
	{
		public Character() 
		{
			Parents = new List<Character>();
			Spouses = new List<Character>();
			Siblings = new List<Character>();
			Children = new List<Character>();
			//Race = new Race(new RacePreset());
			//CharacterName = new CharacterName("FORMAT");
			CurrentInventory = new Inventory();
		}
		public Character(int age, Gender gender, Race race, Faction faction, int politicalPriority, int economicPriority,
			int religiousPriority, int militaryPriority, int influence, int wealth, int strength, int dexterity,
			int constitution, int intelligence, int wisdom, int charisma, bool majorCharacter, List<Character> parents = null, Inventory inventory = null)
		{
			Age = age;
			AffiliatedRace = race;
			Gender = gender == Gender.ANY ? (Gender)(SimRandom.RandomRange(0, 2)) : gender;
			AffiliatedFaction = faction;
			//PoliticalPriority = politicalPriority;
			//EconomicPriority = economicPriority;
			//ReligiousPriority = religiousPriority;
			//MilitaryPriority = militaryPriority;
			Priorities = new Dictionary<OrganizationType, int>();
			Priorities[OrganizationType.POLITICAL] = politicalPriority;
			Priorities[OrganizationType.ECONOMIC] = economicPriority;
			Priorities[OrganizationType.RELIGIOUS] = religiousPriority;
			Priorities[OrganizationType.MILITARY] = militaryPriority;
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
			CurrentInventory = inventory == null ? new Inventory() : inventory;
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
			AffiliatedRace = affiliatedFaction.MajorityRace;
			Age = SimRandom.RandomRange(AffiliatedRace.MinAge, AffiliatedRace.MaxAge);
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
		public Race AffiliatedRace { get; set; }
		public Faction AffiliatedFaction { get; set; }
		public Organization Organization { get; set; }
		public OrganizationPosition OfficialPosition => GetOfficialPosition();
		public int PoliticalPriority
		{
			get { return Priorities[OrganizationType.POLITICAL]; }
			set { Priorities[OrganizationType.POLITICAL] = value; }
		}
		public int EconomicPriority
		{
			get { return Priorities[OrganizationType.ECONOMIC]; }
			set { Priorities[OrganizationType.ECONOMIC] = value; }
		}
		public int ReligiousPriority
		{
			get { return Priorities[OrganizationType.RELIGIOUS]; }
			set { Priorities[OrganizationType.RELIGIOUS] = value; }
		}
		public int MilitaryPriority
		{
			get { return Priorities[OrganizationType.MILITARY]; }
			set { Priorities[OrganizationType.MILITARY] = value; }
		}

		//IMPORTANT! : Need to update the read/write saving for this class to account for new properties
		public Dictionary<OrganizationType, int> Priorities { get; set; }
		public int Influence { get; set; }
		public int Wealth { get; set; }
		public int Strength { get; set; }
		public int Dexterity { get; set; }
		public int Constitution { get; set; }
		public int Intelligence { get; set; }
		public int Wisdom { get; set; }
		public int Charisma { get; set; }
		public Inventory CurrentInventory { get; set; }
		public List<Character> Parents { get; set; }
		public List<Character> Spouses { get; set; }
		public List<Character> Siblings { get; set; }
		public List<Character> Children { get; set; }

		//public List<Character> Family => new List<Character>().Union(Parents).Union(Spouses).Union(Siblings).Union(Children).ToList();
		public List<Character> Family => CharacterExtensions.GetExtendedFamily(this);

		public OrganizationType PriorityAlignment => GetHighestPriority();
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
			var child = new Character(childAge, Enums.Gender.ANY, AffiliatedRace, AffiliatedFaction, majorPlayer, parents);
			Children.Add(child);

			return child;
		}

		public void GenerateFamily(bool generateParents, bool canGenerateSpouse)
		{
			if(generateParents)
			{
				if(Parents.Count(x => x.Gender == Gender.MALE) < 1)
				{
					var father = new Character(Gender.MALE, AffiliatedRace, AffiliatedFaction, false);
					Parents.Add(father);
					ContextDictionaryProvider.AddContext(father);
				}
				if(Parents.Count(x => x.Gender == Gender.FEMALE) < 1)
				{
					var mother = new Character(Gender.FEMALE, AffiliatedRace, AffiliatedFaction, false);
					Parents.Add(mother);
					ContextDictionaryProvider.AddContext(mother);
				}
			}
			if(canGenerateSpouse && Spouses.Count == 0)
			{
				if(SimRandom.RandomBool())
				{
					var gender = Gender == Gender.MALE ? Gender.FEMALE : Gender.MALE;
					var spouse = new Character(gender, AffiliatedRace, AffiliatedFaction, false);
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
					var sibling = new Character(Gender.ANY, AffiliatedRace, AffiliatedFaction, false, Parents);
					Siblings.Add(sibling);
					ContextDictionaryProvider.AddContext(sibling);
				}
			}
		}

		override public void Die()
		{
			if(Children.Count > 0)
			{
				foreach(var item in CurrentInventory.Items)
				{
					SimRandom.RandomEntryFromList(Children).CurrentInventory.Items.Add(item);
				}

				CurrentInventory.Items.Clear();
			}
			else if(AffiliatedFaction.Cities.Count > 0)
			{
				SimRandom.RandomEntryFromList(AffiliatedFaction.Cities).CurrentInventory.Items.AddRange(CurrentInventory.Items);
				CurrentInventory.Items.Clear();
			}
			else
			{
				SimulationManager.Instance.world.LostItems.AddRange(CurrentInventory.Items);
				CurrentInventory.Items.Clear();
			}

			if(MajorCharacter)
			{
				IncidentService.Instance.ReportStaticIncident("{0} dies.", new List<IIncidentContext>() { this });
			}
			EventManager.Instance.Dispatch(new RemoveContextEvent(this));
		}

		public override void LoadContextProperties()
		{
			AffiliatedRace = SaveUtilities.ConvertIDToContext<Race>(contextIDLoadBuffers["Race"][0]);
			AffiliatedFaction = SaveUtilities.ConvertIDToContext<Faction>(contextIDLoadBuffers["AffiliatedFaction"][0]);
			Parents = SaveUtilities.ConvertIDsToContexts<Character>(contextIDLoadBuffers["Parents"]);
			Spouses = SaveUtilities.ConvertIDsToContexts<Character>(contextIDLoadBuffers["Spouses"]);
			Siblings = SaveUtilities.ConvertIDsToContexts<Character>(contextIDLoadBuffers["Siblings"]);
			Children = SaveUtilities.ConvertIDsToContexts<Character>(contextIDLoadBuffers["Children"]);
			CurrentInventory?.LoadContextProperties();

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

		private OrganizationType GetHighestPriority()
		{
			return Priorities.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
		}
	}
}