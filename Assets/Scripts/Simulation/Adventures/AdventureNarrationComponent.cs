using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureNarrationComponent : AdventureComponent
	{
		[TextArea(15, 20), PropertyOrder(0), Title("Narration Text")]
		public string text;
	}
}
