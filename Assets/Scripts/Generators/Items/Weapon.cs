using Game.Data.EventHandling.EventRecording;
using Game.Enums;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Game.Generators.Items
{
	public class Weapon : Item, IEquipable
	{
		public WeaponType type;
		public int flatBonus;
		public ItemGrade itemGrade;

		public Weapon(string name, Material material, WeaponType baseStats, int flatBonus, ItemGrade itemGrade, List<string> gameEffects, List<MethodInfo> simulationEffects) : base (name, material, simulationEffects, gameEffects)
		{
			this.type = baseStats;
			this.flatBonus = flatBonus;
			this.itemGrade = itemGrade;
		}

		public override string Name => GetWeaponName();

		private string GetWeaponName()
		{
			var weaponName = name;
			if(flatBonus > 0)
			{
				weaponName = string.Format("+{0} {1}", flatBonus, name);
			}
			return weaponName;
		}
	}
}