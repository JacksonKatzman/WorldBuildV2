using Game.Simulation;
using Sirenix.OdinInspector;
using System;

namespace Game.Incidents
{
	public class GetOrCreateLandmarkAction : GetOrCreateAction<Landmark>
	{
		[ShowIf("@this.allowCreate")]
		public LocationActionField location;

		protected override void MakeNew()
		{
			var newLandmark = new Landmark(location.GetTypedFieldValue());
			SimulationManager.Instance.world.AddContext(newLandmark);
			result.SetValue(newLandmark);
		}
	}
}