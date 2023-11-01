using Game.Debug;
using Game.Simulation;

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

			EventManager.Instance.Dispatch(new CityChangedControlEvent(gainer, loser, c));

			if (loser.Cities.Count > 0)
			{
				EventManager.Instance.Dispatch(new TerritoryChangedControlEvent(gainer, loser, c.CurrentLocation));
			}
			else
			{
				foreach(var index in loser.ControlledTileIndices)
				{
					EventManager.Instance.Dispatch(new TerritoryChangedControlEvent(gainer, loser, new Location(index)));
				}
				OutputLogger.Log($">>>>{loser.Name} is wiped out by {gainer.Name}");
			}
		}
	}
}