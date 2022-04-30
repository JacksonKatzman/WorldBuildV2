using Game.Enums;
using System.Collections;
using UnityEngine;

namespace Game.Generators.Items
{
	public class Armor : MonoBehaviour
	{
		public ArmorType type;
		public int flatBonus;
		public ItemGrade itemGrade;

		public int GetArmorValue(Person person)
		{
			return type.baseArmor + Mathf.Clamp(person.stats.GetModValue(type.mod), type.modMin, type.modMax) + flatBonus;
		}
	}
}