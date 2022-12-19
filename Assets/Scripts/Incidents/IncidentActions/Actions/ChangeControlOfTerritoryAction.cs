using Game.Simulation;

namespace Game.Incidents
{
	public class ChangeControlOfTerritoryAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Faction> territoryGainer;
		public ContextualIncidentActionField<Faction> territoryLoser;
		public LocationActionField location;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var gainer = territoryGainer.GetTypedFieldValue();
			var loser = territoryLoser.GetTypedFieldValue();
			var tileIndex = location.GetTypedFieldValue().TileIndex;

			if (!gainer.ControlledTileIndices.Contains(tileIndex))
			{
				if(SimulationUtilities.FindSharedBorderFaction(gainer).Contains(tileIndex))
				{
					gainer.ControlledTileIndices.Add(tileIndex);
				}
			}
			if(loser.ControlledTileIndices.Contains(tileIndex))
			{
				loser.ControlledTileIndices.Remove(tileIndex);
			}

			//Might need to notify tile inhabitants later so they can adjust for the change
		}
	}
}