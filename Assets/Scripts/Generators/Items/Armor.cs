using Game.Enums;
using UnityEngine;

namespace Game.Generators.Items
{
	public class Armor : Item, IEquipable
	{
		public ArmorType type;
		public int flatBonus;
		public ItemGrade itemGrade;

		public Armor()
		{
		}
	}
}