using Game.Enums;
using UnityEngine;

namespace Game.Generators.Items
{
	[CreateAssetMenu(fileName = nameof(ArmorType), menuName = "ScriptableObjects/Items/Types/" + nameof(ArmorType), order = 1)]
	public class ArmorType : ScriptableObject
	{
		new public string name;
		public ArmorCategory category;
		public ItemValue value;
		public int baseArmor;
		public StatType mod = StatType.DEXTERITY;
		public int modMin = -100;
		public int modMax = 100;
		public int strengthRequirement;
		public bool stealthDisadvantage;
		public int weight;
	}
}