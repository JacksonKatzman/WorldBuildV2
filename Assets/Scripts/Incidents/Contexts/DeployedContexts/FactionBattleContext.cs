using Game.Simulation;
using Sirenix.OdinInspector;
using System.Linq;

namespace Game.Incidents
{
	public class FactionBattleContext : DeployableContext
	{
		[HideReferenceObjectPicker]
		public DeployedContextActionField<Faction> attacker;
		[HideReferenceObjectPicker]
		public DeployedContextActionField<Faction> defender;

		public bool ShareABorder => CheckIfSharedBorder();
		public bool DefenderHasCityOnBorder => CheckIfCityOnBorder();

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

		private bool CheckIfCityOnBorder()
		{
			var outsideAttackerBorder = SimulationUtilities.FindBorderOutsideFaction(attacker.GetTypedFieldValue());
			var cityCells = defender.GetTypedFieldValue().Cities.Select(x => x.CurrentLocation.TileIndex).ToList();
			foreach(var cityCell in cityCells)
			{
				if(outsideAttackerBorder.Contains(cityCell))
				{
					return true;
				}
			}
			return false;
		}
	}
}