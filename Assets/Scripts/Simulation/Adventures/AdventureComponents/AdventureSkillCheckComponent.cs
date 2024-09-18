using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Game.Simulation
{
	public class AdventureSkillCheckComponent : AdventureComponent
	{
		//skill check type
		//difficulty
		[GUIColor("@successLink > -1 ? Color.green : Color.red")]
		public int successLink = -1;
		[GUIColor("@failureLink > -1 ? Color.green : Color.red")]
		public int failureLink = -1;
	}
}
