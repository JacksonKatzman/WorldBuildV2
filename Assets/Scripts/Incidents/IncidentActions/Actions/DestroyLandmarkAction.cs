using Game.Simulation;

namespace Game.Incidents
{
	public class DestroyLandmarkAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Landmark> landmark;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			SimulationManager.Instance.world.RemoveContext(landmark.GetTypedFieldValue());
		}
	}
}