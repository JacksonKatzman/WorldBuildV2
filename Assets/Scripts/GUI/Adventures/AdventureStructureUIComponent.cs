using Game.Simulation;
using System.Collections.Generic;
using TMPro;

namespace Game.GUI.Wiki
{
	public class AdventureStructureUIComponent : AdventureUIComponent<AdventureStructureComponent>
	{
		public TMP_Text structureNameText;
		public TMP_Text structureDescription;
		public TMP_Text ceilingDescription;
		public TMP_Text floorsAndWallsDescription;
		public TMP_Text doorsDescription;
		public TMP_Text lightingDescription;

		protected override List<TMP_Text> AssociatedTexts => new List<TMP_Text>() { structureNameText, structureDescription,
			ceilingDescription, floorsAndWallsDescription, doorsDescription, lightingDescription };

		public override void BuildUIComponents(AdventureStructureComponent component)
		{
			throw new System.NotImplementedException();
		}
	}
}
