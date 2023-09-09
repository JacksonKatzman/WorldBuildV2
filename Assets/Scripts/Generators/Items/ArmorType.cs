using Game.Enums;
using UnityEngine;

namespace Game.Generators.Items
{
	[CreateAssetMenu(fileName = nameof(ArmorType), menuName = "ScriptableObjects/Items/Types/" + nameof(ArmorType), order = 1)]
	public class ArmorType : ItemType
	{
		public ArmorCategory category;
		public int baseArmor;
		public StatType mod = StatType.DEXTERITY;
		public int modMin = -100;
		public int modMax = 100;
		public int strengthRequirement;
		public bool stealthDisadvantage;
	}
}