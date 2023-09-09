using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureStructureComponent : AdventureComponent
	{
		public string structureName;
		[TextArea(5, 10)]
		public string structureDescription;
		[TextArea(3, 6)]
		public string ceilingDescription;
		[TextArea(3, 6)]
		public string floorsAndWallsDescription;
		[TextArea(3, 6)]
		public string doorsDescription;
		[TextArea(3, 6)]
		public string lightingDescription;

		override public void UpdateContextIDs(List<int> removedIds = null)
		{
			structureName = EncounterEditorWindow.UpdateInTextIDs(structureName, removedIds);
			structureDescription = EncounterEditorWindow.UpdateInTextIDs(structureDescription, removedIds);
			ceilingDescription = EncounterEditorWindow.UpdateInTextIDs(ceilingDescription, removedIds);
			floorsAndWallsDescription = EncounterEditorWindow.UpdateInTextIDs(floorsAndWallsDescription, removedIds);
			doorsDescription = EncounterEditorWindow.UpdateInTextIDs(doorsDescription, removedIds);
			lightingDescription = EncounterEditorWindow.UpdateInTextIDs(lightingDescription, removedIds);
		}
	}
}
