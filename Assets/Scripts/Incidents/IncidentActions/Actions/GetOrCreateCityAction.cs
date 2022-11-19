using Game.Simulation;

namespace Game.Incidents
{
	public class GetOrCreateCityAction : GetOrCreateAction<City>
	{
		public ContextualIncidentActionField<Faction> faction;
		public LocationActionField location;
		public IntegerRange population;
		public IntegerRange wealth;
		protected override void MakeNew()
		{
			var newCity = new City(faction.GetTypedFieldValue(), location.GetTypedFieldValue(), population, wealth);
			faction.GetTypedFieldValue().Cities.Add(newCity);
			SimulationManager.Instance.world.AddContext(newCity);
			result.SetValue(newCity);
			OutputLogger.Log("City created!");
		}
	}
}