using Game.Simulation;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;

namespace Game.GUI.Adventures
{
	public class AdventureTitleUIComponent : AdventureSingleTextUIComponent<AdventureTitleComponent>
	{
		public override void ReplaceTextPlaceholders(List<IAdventureContextCriteria> contexts)
		{
			base.ReplaceTextPlaceholders(contexts);
			AdventureGuide.Instance.CreateTableOfContentsEntry(ComponentID, text.text);
		}
	}
}
