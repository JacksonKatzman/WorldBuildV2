using Game.Debug;

namespace Game.Incidents
{
	public class ChangeControlOfCityAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Faction> cityGainer;
		public ContextualIncidentActionField<Faction> cityLoser;
		public ContextualIncidentActionField<City> city;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var gainer = cityGainer.GetTypedFieldValue();
			var loser = cityLoser.GetTypedFieldValue();
			var c = city.GetTypedFieldValue();
			var tileIndex = c.CurrentLocation.TileIndex;

			if(!gainer.Cities.Contains(c))
			{
				gainer.Cities.Add(c);
			}
			if(loser.Cities.Contains(c))
			{
				loser.Cities.Remove(c);
			}
			if (loser.Cities.Count > 0)
			{
				if (!gainer.ControlledTileIndices.Contains(tileIndex))
				{
					gainer.ControlledTileIndices.Add(tileIndex);
				}
				if (loser.ControlledTileIndices.Contains(tileIndex))
				{
					loser.ControlledTileIndices.Remove(tileIndex);
				}
			}
			else
			{
				gainer.ControlledTileIndices.AddRange(loser.ControlledTileIndices);
				OutputLogger.Log($">>>>{loser.Name} is wiped out by {gainer.Name}");
			}

			c.AffiliatedFaction = gainer;
			gainer.ClaimTerritoryBetweenCities();
			//Might need to notify tile inhabitants later so they can adjust for the change
		}
	}
}