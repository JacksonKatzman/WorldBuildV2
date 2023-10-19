using Game.Debug;
using Game.Simulation;

namespace Game.Incidents
{
	public class KillCharacterAction : GenericIncidentAction
	{
		//public InterfacedIncidentActionFieldContainer<ICharacter> person;
		public InterfacedIncidentActionFieldContainer<ISentient> person;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			person.GetTypedFieldValue().Die();
			OutputLogger.Log("Character Killed!");
		}
	}
}