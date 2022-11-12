using Game.Simulation;
using System;

namespace Game.Incidents
{
	public class CreateLandmarkAction : GenericIncidentAction
	{
		public LocationActionField location;
		public ActionResultField<Landmark> resultLandmark;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var landmark = new Landmark(location.GetTypedFieldValue());
			SimulationManager.Instance.world.AddContext(landmark);
			resultLandmark.SetValue(landmark);
		}
	}
}