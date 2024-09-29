using Game.Simulation;
using System.Collections.Generic;
using TMPro;

namespace Game.GUI.Adventures
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
			structureNameText.text = component.structureName.Text;
			structureDescription.text = component.structureDescription.Text;
			ceilingDescription.text = $"<b>Ceiling:</b> {component.ceilingDescription.Text}";
			floorsAndWallsDescription.text = $"<b>Floors and Walls:</b> {component.floorsAndWallsDescription.Text}";
			doorsDescription.text = $"<b>Doors:</b> {component.doorsDescription.Text}";
			lightingDescription.text = $"<b>Lighting:</b> {component.lightingDescription.Text}";
		}
	}
}
