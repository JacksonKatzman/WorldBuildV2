using Game.Simulation;
using System;

namespace Game.Incidents
{
	public class GetOrCreateLandmarkAction : GenericIncidentAction
	{
		public ContextualIncidentActionField<Landmark> landmark;

		public LocationActionField location;
		public ActionResultField<Landmark> resultLandmark;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			if (landmark.GetTypedFieldValue() == null)
			{
				var newLandmark = new Landmark(location.GetTypedFieldValue());
				SimulationManager.Instance.world.AddContext(newLandmark);
				resultLandmark.SetValue(newLandmark);
			}
			else
			{
				resultLandmark.SetValue(landmark.GetTypedFieldValue());
			}
		}
	}
}