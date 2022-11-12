using Game.Enums;
using System.Collections.Generic;
using System.Reflection;

namespace Game.Generators.Items
{
	public class Weapon : Item, IEquipable
	{
		public WeaponType type;
		public int flatBonus;
		public ItemGrade itemGrade;

		public Weapon()
		{
		}
	}
}