using Game.Simulation;

namespace Game.Incidents
{
	public class GetOrCreateCityAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<City> city;

		public ContextualIncidentActionField<Faction> faction;
		public LocationActionField location;
		public IntegerRange population;
		public IntegerRange wealth;
		public ActionResultField<City> resultCity;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			if (city.GetTypedFieldValue() == null)
			{
				var newCity = new City(faction.GetTypedFieldValue(), location.GetTypedFieldValue(), population, wealth);
				faction.GetTypedFieldValue().Cities.Add(newCity);
				SimulationManager.Instance.world.AddContext(newCity);
				resultCity.SetValue(newCity);
				OutputLogger.Log("City created!");
			}
			else
			{
				resultCity.SetValue(city.GetTypedFieldValue());
			}
		}
	}

	//public class 
}