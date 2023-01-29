using Game.Creatures;
using Game.Simulation;
using System;

namespace Game.Incidents
{
	public class GreatMonster : IncidentContext, IInventoryAffiliated, IAlignmentAffiliated
	{
		public MonsterData dataBlock;
		public Inventory Inventory { get; private set; }

		public int LawfulChaoticAlignmentAxis { get; set; }
		public int GoodEvilAlignmentAxis { get; set; }

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