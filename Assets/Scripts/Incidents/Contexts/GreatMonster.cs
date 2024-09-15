using Game.Data;
using Game.Enums;
using Game.Generators.Names;
using Game.Simulation;
using Game.Utilities;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class GreatMonster : IncidentContext, IInventoryAffiliated, IAlignmentAffiliated, IFactionAffiliated, ISentient
	{
		public MonsterData dataBlock;
		public override string Name => CharacterName.FullName;

		public Inventory CurrentInventory { get; set; }
		public CharacterName CharacterName { get; set; }
		public Gender Gender { get; set; }

		public OrganizationType PriorityAlignment => OrganizationType.MILITARY;
		public int LawfulChaoticAlignmentAxis { get; set; }
		public int GoodEvilAlignmentAxis { get; set; }
		public List<CharacterTrait> CharacterTraits { get; set; }
		public Faction AffiliatedFaction { get; set; }
		public IOrganizationPosition OrganizationPosition { get; set; }
		public Organization AffiliatedOrganization => OrganizationPosition.AffiliatedOrganization;

		public CreatureSize CreatureSize => dataBlock.size;
		public CreatureType CreatureType => dataBlock.type;

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

        public override string Description => $"{Age.ToString()}, {Gender.ToString()}, Legendary {dataBlock.size.ToString()} {dataBlock.type.ToString()}, {dataBlock.alignment.ToString()}";

        public IIncidentContext Context => this;

        public GreatMonster()// : base()
		{
			CharacterTraits = new List<CharacterTrait>();
			Priorities = new Dictionary<OrganizationType, int>();
			Priorities[OrganizationType.POLITICAL] = 0;
			Priorities[OrganizationType.ECONOMIC] = 0;
			Priorities[OrganizationType.RELIGIOUS] = 0;
			Priorities[OrganizationType.MILITARY] = 0;
		}
		public GreatMonster(MonsterData dataBlock) : this()
		{
			this.dataBlock = dataBlock;
		}

		public void TransformFrom(Character character)
		{
			CharacterName = character.CharacterName;
			character.Die();
		}

		public override void DeployContext()
		{

		}

		public override void Die()
		{
			EventManager.Instance.Dispatch(new RemoveContextEvent(this, GetType()));
		}

		public override void UpdateContext()
		{
			
		}

		public override void CheckForDeath()
		{
			
		}

		public override void LoadContextProperties()
		{
			AffiliatedFaction = SaveUtilities.ConvertIDToContext<Faction>(contextIDLoadBuffers["AffiliatedFaction"][0]);

			CurrentInventory.LoadContextProperties();

			contextIDLoadBuffers.Clear();
		}
	}
}