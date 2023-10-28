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
			GameProfiler.BeginProfiling("KillCharacterAction", GameProfiler.ProfileFunctionType.DEPLOY);
			person.GetTypedFieldValue().Die();
			GameProfiler.EndProfiling("KillCharacterAction");
			OutputLogger.Log("Character Killed!");
		}
	}
}