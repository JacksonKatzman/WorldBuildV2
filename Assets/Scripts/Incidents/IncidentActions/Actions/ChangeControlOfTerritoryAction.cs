using Game.Debug;
using Game.Simulation;
using System.Linq;

namespace Game.Incidents
{
	public class ChangeControlOfTerritoryAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Faction> territoryGainer;
		public ContextualIncidentActionField<Faction> territoryLoser;
		public LocationActionField location;

		public override bool VerifyAction(IIncidentContext context)
		{
			var verify = base.VerifyAction(context);
			if(ContextDictionaryProvider.GetCurrentContexts<City>().Where(x => x.CurrentLocation.TileIndex == location.GetTypedFieldValue().TileIndex).Count() > 0)
			{
				verify = false;
				OutputLogger.LogWarning("Tried to change control of territory that contains city.");
			}
			return verify;
		}

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var gainer = territoryGainer.GetTypedFieldValue();
			var loser = territoryLoser.GetTypedFieldValue();
			var loc = location.GetTypedFieldValue();

			EventManager.Instance.Dispatch(new TerritoryChangedControlEvent(gainer, loser, loc));

			/*
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
			*/

			//Might need to notify tile inhabitants later so they can adjust for the change
		}
	}
}