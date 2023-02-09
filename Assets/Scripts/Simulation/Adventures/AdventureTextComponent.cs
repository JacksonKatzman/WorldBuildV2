using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureTextComponent : AdventureComponent
	{
		[Title("Descriptive/Background Text")]
		public string title;

		[TextArea(15, 20), PropertyOrder(0)]
		public string text;
	}
}
