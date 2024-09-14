namespace Game.Generators.Items
{
	public class Trinket : Item, IEquipable
	{
		public float ValueMultiplier { get; set; }
		public override string Description => $"TRINKET DESCRIPTION";
		public Trinket()
		{

		}

		public override void RollStats(int points)
		{
			
		}
	}
}