using Game.Simulation;
using System;

namespace Game.Incidents
{
	public class CreateCityAction : GenericIncidentAction
	{
		ContextualIncidentActionField<Faction> faction;
		public IntegerRange population;
		public IntegerRange wealth;
		public ActionResultField<City> resultCity;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var city = new City(faction.GetTypedFieldValue(), population, wealth);
			SimulationManager.Instance.world.AddContext(city);
			resultCity.SetValue(city);
			OutputLogger.Log("City created!");
		}
	}
}