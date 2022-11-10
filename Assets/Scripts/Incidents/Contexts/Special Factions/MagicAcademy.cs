using System;

namespace Game.Incidents
{
	public class MagicAcademy : SpecialFaction
	{
		public override bool CanExpandTerritory => false;
		public override bool CanTakeMilitaryAction => false;
	}
}