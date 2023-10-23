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
		public override string Name => CharacterName.fullName;
		public Inventory CurrentInventory { get; set; }
		public CharacterName CharacterName { get; set; }

		public OrganizationType PriorityAlignment => OrganizationType.MILITARY;
		public int LawfulChaoticAlignmentAxis { get; set; }
		public int GoodEvilAlignmentAxis { get; set; }
		public List<CharacterTag> CharacterTags { get; set; }
		public Faction AffiliatedFaction { get; set; }

		public CreatureSize CreatureSize => dataBlock.size;
		public CreatureType CreatureType => dataBlock.type;

		public GreatMonster() 
		{
			CharacterTags = new List<CharacterTag>();
		}
		public GreatMonster(MonsterData dataBlock) : base()
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