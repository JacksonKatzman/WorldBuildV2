using Game.Simulation;

namespace Game.Incidents
{
	public class DestroyFactionAction : GenericIncidentAction
	{
        public ContextualIncidentActionField<Faction> faction;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			EventManager.Instance.Dispatch(new RemoveContextEvent(faction.GetTypedFieldValue()));
		}
	}
}