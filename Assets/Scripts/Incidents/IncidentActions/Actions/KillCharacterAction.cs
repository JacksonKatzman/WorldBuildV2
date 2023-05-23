using Game.Simulation;

namespace Game.Incidents
{
	public class KillCharacterAction : GenericIncidentAction
	{
		public InterfacedIncidentActionFieldContainer<ICharacter> person;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			person.GetTypedFieldValue().Die();
			OutputLogger.Log("Character Killed!");
		}
	}
}