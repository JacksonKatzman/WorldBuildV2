using Game.Combat;
using Game.Enums;
using UnityEngine;

namespace Game.Generators.Items
{
	[CreateAssetMenu(fileName = nameof(WeaponType), menuName = "ScriptableObjects/Items/Types/" + nameof(WeaponType), order = 1)]
	public class WeaponType : ItemType
	{
		public WeaponCategory category;
		public DamageValue damageValue;
		public string properties;
	}
}