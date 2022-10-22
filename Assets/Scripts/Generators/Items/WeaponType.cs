using Game.Combat;
using Game.Enums;
using UnityEngine;

namespace Game.Generators.Items
{
	[CreateAssetMenu(fileName = nameof(WeaponType), menuName = "ScriptableObjects/Items/Types/" + nameof(WeaponType), order = 1)]
	public class WeaponType : ScriptableObject
	{
		public string Name => name;

		new public string name;
		public WeaponCategory category;
		public ItemValue value;
		public DamageValue damageValue;
		public float weight;
		public string properties;
	}
}