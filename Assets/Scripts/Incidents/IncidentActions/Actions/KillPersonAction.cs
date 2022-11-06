using Game.Simulation;

namespace Game.Incidents
{
	public class KillPersonAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Person> person;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			SimulationManager.Instance.world.RemoveContext(person.GetTypedFieldValue());
			OutputLogger.Log("Person Killed!");
		}
	}
}