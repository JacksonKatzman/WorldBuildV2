using Game.Simulation;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;

namespace Game.GUI.Wiki
{
	public class AdventureTitleUIComponent : AdventureSingleTextUIComponent
	{
		public override void ReplaceTextPlaceholders(List<IAdventureContextCriteria> contexts)
		{
			base.ReplaceTextPlaceholders(contexts);
			AdventureGuide.Instance.CreateTableOfContentsEntry(ComponentID, text.text);
		}
	}
}
