using Game.Enums;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Generators.Items
{
	public class Weapon : Item, IEquipable
	{
		public WeaponType WeaponType { get; set; }
		public int FlatBonus { get; set; }
		public ItemGrade ItemGrade { get; set; }
		public override string Description => $"WEAPON DESCRIPTION";

		public Weapon()
		{
		}

		public Weapon(WeaponType weaponType)
		{
			this.WeaponType = weaponType;
		}

		public override void RollStats(int points)
		{
			
		}
	}
}