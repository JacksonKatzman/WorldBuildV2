using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
	public class AdventureStructureComponent : AdventureComponent
	{
		public AdventureComponentTextField structureName = new AdventureComponentTextField();
		public AdventureComponentTextField structureDescription = new AdventureComponentTextField();
		public AdventureComponentTextField ceilingDescription = new AdventureComponentTextField();
		public AdventureComponentTextField floorsAndWallsDescription = new AdventureComponentTextField();
		public AdventureComponentTextField doorsDescription = new AdventureComponentTextField();
		public AdventureComponentTextField lightingDescription = new AdventureComponentTextField();
	}
}
