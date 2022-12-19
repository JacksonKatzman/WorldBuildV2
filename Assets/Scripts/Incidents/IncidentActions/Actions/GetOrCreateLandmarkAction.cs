using Game.Simulation;
using Sirenix.OdinInspector;
using System;

namespace Game.Incidents
{
	public class GetOrCreateLandmarkAction : GetOrCreateAction<Landmark>
	{
		[ShowIf("@this.allowCreate")]
		public LocationActionField location;

		protected override Landmark MakeNew()
		{
			var newLandmark = new Landmark(location.GetTypedFieldValue());

			return newLandmark;
		}

		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			return location.CalculateField(context);
		}
	}
}