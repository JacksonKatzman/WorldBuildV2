using Game.Creatures;
using Game.Enums;
using Game.Generators.Names;
using Game.Simulation;
using Game.Utilities;
using System;

namespace Game.Incidents
{
	public class GreatMonster : IncidentContext, IInventoryAffiliated, IAlignmentAffiliated, IFactionAffiliated
	{
		public MonsterData dataBlock;
		public override string Name => CharacterName.fullName;
		public Inventory Inventory { get; private set; }
		public CharacterName CharacterName { get; set; }

		public int LawfulChaoticAlignmentAxis { get; set; }
		public int GoodEvilAlignmentAxis { get; set; }

		public Faction AffiliatedFaction { get; set; }

		public CreatureSize CreatureSize => dataBlock.size;
		public CreatureType CreatureType => dataBlock.type;

		public GreatMonster() { }
		public GreatMonster(MonsterData dataBlock)
		{
			this.dataBlock = dataBlock;
		}

		public GreatMonster(MonsterData dataBlock, Character person) : this(dataBlock)
		{
			Name = person.Name;
			person.Die();
		}

		public override void DeployContext()
		{

		}

		public override void Die()
		{
			EventManager.Instance.Dispatch(new RemoveContextEvent(this));
		}

		public override void UpdateContext()
		{
			
		}

		public override void LoadContextProperties()
		{
			AffiliatedFaction = SaveUtilities.ConvertIDToContext<Faction>(contextIDLoadBuffers["AffiliatedFaction"][0]);

			Inventory.LoadContextProperties();

			contextIDLoadBuffers.Clear();
		}
	}
}