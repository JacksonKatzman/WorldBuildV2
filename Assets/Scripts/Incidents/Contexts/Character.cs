using Game.Debug;
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
	public class Character : IncidentContext, ICharacter, IFactionAffiliated, IInventoryAffiliated, IAlignmentAffiliated, IRaceAffiliated, IOrganizationAffiliated, ILocationAffiliated
	{
		public Character() 
		{
			Parents = new List<Character>();
			Spouses = new List<Character>();
			Siblings = new List<Character>();
			Children = new List<Character>();
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
			CharacterTraits = new List<CharacterTrait>();

			if(Parents.Count > 0)
			{
				CharacterName = AffiliatedFaction?.namingTheme.GenerateSentientName(Gender, parents);
			}
			else
			{
				CharacterName = AffiliatedFaction?.namingTheme.GenerateSentientName(Gender);
			}

			EventManager.Instance.AddEventHandler<AffiliatedFactionChangedEvent>(OnFactionChangeEvent);
			EventManager.Instance.AddEventHandler<AffiliatedOrganizationChangedEvent>(OnOrganizationChangeEvent);
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
			CharacterName = affiliatedFaction.namingTheme.GenerateSentientName(Gender);
			MajorCharacter = false;
			Parents = new List<Character>();
			Spouses = new List<Character>();
			Siblings = new List<Character>();
			Children = new List<Character>();
			CharacterTraits = new List<CharacterTrait>();
		}

		public override string Name => CharacterName.GetTitledFullName(this);
		public CharacterName CharacterName { get; set; }
		public Gender Gender { get; set; }
		public Race AffiliatedRace { get; set; }
		public Faction AffiliatedFaction { get; set; }
		public Organization AffiliatedOrganization
		{
			get
			{
				return OrganizationPosition.AffiliatedOrganization;
			}
			set
			{
				OutputLogger.LogWarning("Cannot directly set Character Organization.");
			}
		}
		public IOrganizationPosition OrganizationPosition { get; set; }
		public bool HasOrganizationPosition => OrganizationPosition != null;
		public bool HasGovernmentPosition => HasOrganizationPosition && OrganizationPosition.AffiliatedOrganization == AffiliatedFaction.Government;
		public bool IsPrimaryPositionHolder => HasOrganizationPosition && OrganizationPosition.PrimaryOfficial == this;
		public int OrganizationTier => HasOrganizationPosition ? OrganizationPosition.OrganizationTier : int.MaxValue;
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
		public List<CharacterTrait> CharacterTraits { get; set; }

		//public List<Character> Family => new List<Character>().Union(Parents).Union(Spouses).Union(Siblings).Union(Children).ToList();
		public List<Character> Family => CharacterExtensions.GetExtendedFamily(this);
		public int LivingFamilyMembers => CountLivingFamilyMembers();
		public int SpouseCount => Spouses.Count;

		public OrganizationType PriorityAlignment => GetHighestPriority();
		public int LawfulChaoticAlignmentAxis { get; set; }
		public int GoodEvilAlignmentAxis { get; set; }

		public bool MajorCharacter { get; set; }
		public bool Possessed { get; set; }

        public Location CurrentLocation { get; set; }
		public override string Description => $"{Age.ToString()}, {Gender.ToString()}, {AffiliatedRace.Link()} of {AffiliatedFaction.Link()}";

        public IIncidentContext Context => this;

        override public void DeployContext()
		{
			if (NumIncidents > 0 && MajorCharacter && Age >= 16)
			{
				IncidentService.Instance.PerformIncidents(this);
			}

			CheckForDeath();
		}

		override public void UpdateContext()
		{
			Age += 1;
			NumIncidents = MajorCharacter ? 1 : 0;
		}

		public override void CheckForDeath()
		{
			if (this.CheckDestroyed())
			{
				Die();
			}
		}

		public Character CreateChild(int age, Gender gender, bool majorPlayer)
		{
			var parents = new List<Character>() { this };
			if(SpouseCount > 0)
			{
				parents.Add(SimRandom.RandomEntryFromList(Spouses));
			}

			var child = new Character(age, gender, AffiliatedRace, AffiliatedFaction, majorPlayer, parents);
			Children.Add(child);

			return child;
		}

		public void GenerateFamily(bool generateParents, float spouseChance, int numChildren)
		{
			if(generateParents)
			{
				if(Parents.Count(x => x.Gender == Gender.MALE) < 1)
				{
					var father = new Character(Gender.MALE, AffiliatedRace, AffiliatedFaction, false);
					father.CharacterName = AffiliatedFaction?.namingTheme.GenerateSentientName(Gender.MALE, new List<Character> {this});
					Parents.Add(father);
					father.Children.Add(this);
					EventManager.Instance.Dispatch(new AddContextEvent(father, false));
				}
				if(Parents.Count(x => x.Gender == Gender.FEMALE) < 1)
				{
					var mother = new Character(Gender.FEMALE, AffiliatedRace, AffiliatedFaction, false);
					Parents.Add(mother);
					mother.Children.Add(this);
					EventManager.Instance.Dispatch(new AddContextEvent(mother, false));
				}
				Parents[0]?.Spouses.Add(Parents[1]);
				Parents[1]?.Spouses.Add(Parents[0]);
			}
			if(spouseChance > 0 && Spouses.Count == 0)
			{
				if(SimRandom.RandomFloat01() <= spouseChance)
				{
					var gender = Gender == Gender.MALE ? Gender.FEMALE : Gender.MALE;
					var spouse = new Character(gender, AffiliatedRace, AffiliatedFaction, false);
					Spouses.Add(spouse);
					spouse.Spouses.Add(this);
					EventManager.Instance.Dispatch(new AddContextEvent(spouse, false));
				}
			}
			if(Spouses.Count > 0 && numChildren > 0 && Age >= 16)
			{
				var maxChildAge = Age - 16;
				for(int i = 0; i < numChildren; i++)
				{
					CreateChild(SimRandom.RandomRange(0, maxChildAge), Gender.ANY, true);
				}
			}
			if (Siblings.Count == 0)
			{
				//Change to be a curve later
				var numSiblings = SimRandom.RandomRange(0, 5);
				for (int i = 0; i < numSiblings; i++)
				{
					var sibling = new Character(Gender.ANY, AffiliatedRace, AffiliatedFaction, false, Parents);
					sibling.CharacterName = AffiliatedFaction?.namingTheme.GenerateSentientName(Gender.MALE, Parents);
					Siblings.Add(sibling);
					sibling.Siblings.Add(this);
					EventManager.Instance.Dispatch(new AddContextEvent(sibling, false));
				}
			}
		}

		override public void Die()
		{
			SetPreviousTitle();

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
				IncidentService.Instance.ReportStaticIncident("{0} dies at age " + Age + ".", new List<IIncidentContext>() { this }, true);
			}
			EventManager.Instance.RemoveEventHandler<AffiliatedFactionChangedEvent>(OnFactionChangeEvent);
			EventManager.Instance.RemoveEventHandler<AffiliatedOrganizationChangedEvent>(OnOrganizationChangeEvent);
			EventManager.Instance.Dispatch(new RemoveContextEvent(this, GetType()));
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

		public static void HandleMarriage(Character initiator, Character other)
		{
			initiator.Spouses.Add(other);
			other.Spouses.Add(initiator);
			other.AffiliatedFaction = initiator.AffiliatedFaction;

			EventManager.Instance.Dispatch(new AffiliatedFactionChangedEvent(other, initiator.AffiliatedFaction));

			if(initiator.OrganizationPosition != null)
			{
				//other.OrganizationPosition = initiator.OrganizationPosition;
				initiator.OrganizationPosition.Update();
			}
		}

		private OrganizationType GetHighestPriority()
		{
			return Priorities.Aggregate((x, y) => x.Value > y.Value ? x : y).Key;
		}

		private int CountLivingFamilyMembers()
		{
			var family = Family;
			return family.Where(x => x != this && ContextDictionaryProvider.CurrentContexts[typeof(Character)].Contains(x)).Count();
		}

		private void OnFactionChangeEvent(AffiliatedFactionChangedEvent gameEvent)
		{
			if (gameEvent.affiliate == this)
			{
				SetPreviousTitle();
				OrganizationPosition = null;
			}
		}

		private void OnOrganizationChangeEvent(AffiliatedOrganizationChangedEvent gameEvent)
		{
			if (gameEvent.affiliate == this)
			{
				SetPreviousTitle();
				OrganizationPosition = gameEvent.newPosition;
			}
		}

		private void SetPreviousTitle()
		{
			if (OrganizationPosition != null)
			{
				CharacterName.previousTitle = OrganizationPosition.GetTitle(this);
			}
		}
	}
}