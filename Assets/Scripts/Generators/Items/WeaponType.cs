using Game.Combat;
using Game.Data.EventHandling.EventRecording;
using Game.Enums;
using System.Collections;
using UnityEngine;

namespace Game.Generators.Items
{
	[CreateAssetMenu(fileName = nameof(WeaponType), menuName = "ScriptableObjects/Items/Types/" + nameof(WeaponType), order = 1)]
	public class WeaponType : ScriptableObject, IRecordable
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