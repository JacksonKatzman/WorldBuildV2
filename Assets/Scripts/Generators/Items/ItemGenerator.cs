using System.Collections.Generic;
using System.Reflection;

namespace Game.Generators.Items
{
	public static class ItemGenerator
	{
		public static Item GenerateRelic(List<MaterialUse> uses, int actives)
		{
			return default;
		}
	}

	//I really need a way to take the ScriptableObjects I create for base items and turn them into actual Item contexts.
	//Currently I have no way of matching the two up. I could perhaps use this class and some if/switches to create a factory.
	//The factory would take in the base SO and decide which context to make based on type, but if i can come up with a better way to relate them i want to.
}