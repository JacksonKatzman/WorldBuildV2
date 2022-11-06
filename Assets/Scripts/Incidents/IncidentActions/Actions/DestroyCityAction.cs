using Game.Simulation;

namespace Game.Incidents
{
	public class DestroyCityAction : GenericIncidentAction
	{
		ContextualIncidentActionField<City> city;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			SimulationManager.Instance.world.RemoveContext(city.GetTypedFieldValue());
			OutputLogger.Log("City destroyed!");
		}
	}
}