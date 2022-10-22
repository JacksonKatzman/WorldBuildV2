using System.Collections.Generic;

namespace Game.Generators.Items
{
	public class Material
	{
		public string Name => name;
		public List<MaterialUse> uses;
		public float rarity;

		private string name;
		public Material(string name, List<MaterialUse> uses, float rarity)
		{
			this.name = name;
			this.uses = uses;
			this.rarity = rarity;
		}

		//also add magical effects / properties

	}

	public enum MaterialUse {Construction, Decoration, Forging};
}