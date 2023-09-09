using UnityEngine;

namespace Game.Generators.Items
{
	abstract public class ItemType : ScriptableObject 
	{
		public string Name => name;
		new public string name;
		public float weight;
		public ItemValue value;
	}
}