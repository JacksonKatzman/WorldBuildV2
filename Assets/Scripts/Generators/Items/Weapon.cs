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

		public Weapon()
		{
		}
	}
}