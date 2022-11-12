using Game.Simulation;

namespace Game.Incidents
{
	public class KillPersonAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Person> person;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			person.GetTypedFieldValue().Die();
			OutputLogger.Log("Person Killed!");
		}
	}
}