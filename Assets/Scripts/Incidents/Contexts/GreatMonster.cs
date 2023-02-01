using Game.Creatures;
using Game.Enums;
using Game.Generators.Names;
using Game.Simulation;
using System;

namespace Game.Incidents
{
	public class GreatMonster : IncidentContext, IInventoryAffiliated, IAlignmentAffiliated, IFactionAffiliated
	{
		public MonsterData dataBlock;
		public override string Name => PersonName.fullName;
		public Inventory Inventory { get; private set; }
		public CreatureName PersonName { get; set; }

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

		public GreatMonster(MonsterData dataBlock, Person person) : this(dataBlock)
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
	}
}