using Game.Simulation;

namespace Game.Incidents
{
	public class DestroyCityAction : GenericIncidentAction
	{
		ContextualIncidentActionField<City> city;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			city.GetTypedFieldValue().Die();
			OutputLogger.Log("City destroyed!");
		}
	}
}