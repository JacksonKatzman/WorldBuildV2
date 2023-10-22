using Game.Simulation;
using Sirenix.OdinInspector;

namespace Game.Incidents
{
	public class FactionBattleContext : DeployableContext
	{
		[HideReferenceObjectPicker]
		public DeployedContextActionField<Faction> attacker;
		[HideReferenceObjectPicker]
		public DeployedContextActionField<Faction> defender;

		public bool ShareABorder => CheckIfSharedBorder();

		private bool CheckIfSharedBorder()
		{
			var outsideAttackerBorder = SimulationUtilities.FindBorderOutsideFaction(attacker.GetTypedFieldValue());
			var defenderHexes = defender.GetTypedFieldValue().ControlledTileIndices;
			foreach(var index in outsideAttackerBorder)
			{
				if(defenderHexes.Contains(index))
				{
					return true;
				}
			}
			return false;
		}
	}
}