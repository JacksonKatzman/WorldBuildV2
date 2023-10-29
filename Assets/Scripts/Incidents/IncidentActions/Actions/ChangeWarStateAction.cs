using Game.Debug;
using Game.Simulation;

namespace Game.Incidents
{
	public class ChangeWarStateAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Faction> factionOne;
		public ContextualIncidentActionField<Faction> factionTwo;
		public bool atWar;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var f1 = factionOne.GetTypedFieldValue();
			var f2 = factionTwo.GetTypedFieldValue();
			if(atWar)
			{

				EventManager.Instance.Dispatch(new WarDeclaredEvent(f1, f2));
				OutputLogger.Log("War were declared.");
			}
			else
			{
				EventManager.Instance.Dispatch(new PeaceDeclaredEvent(f1, f2));
				OutputLogger.Log("War were ended.");
			}
		}
	}
}