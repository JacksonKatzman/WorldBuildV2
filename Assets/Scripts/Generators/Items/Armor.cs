using Game.Enums;
using UnityEngine;

namespace Game.Generators.Items
{
	public class Armor : Item, IEquipable
	{
		public ArmorType ArmorType { get; set; }
		public int FlatBonus { get; set; }
		public ItemGrade ItemGrade { get; set; }

		public override string Description => $"ARMOR_DESCRIPTION";

        public Armor()
		{
		}

		public Armor(ArmorType armorType)
		{
			this.ArmorType = armorType;
		}

		public override void RollStats(int points)
		{
			
		}
	}
}