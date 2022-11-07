using Game.Simulation;
using System;

namespace Game.Incidents
{
	public class CreateCityAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Faction> faction;
		public LocationActionField location;
		public IntegerRange population;
		public IntegerRange wealth;
		public ActionResultField<City> resultCity;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var city = new City(faction.GetTypedFieldValue(), location.GetTypedFieldValue(), population, wealth);
			SimulationManager.Instance.world.AddContext(city);
			resultCity.SetValue(city);
			OutputLogger.Log("City created!");
		}
	}
}