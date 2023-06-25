using Game.Simulation;
using Sirenix.OdinInspector;
using System;

namespace Game.Incidents
{
	public class GetOrCreateLandmarkAction : GetOrCreateAction<Landmark>
	{
		[ShowIf("@this.allowCreate")]
		public LocationActionField location;

		//need a scriptable object type for LandmarkStyle that acts as a type and includes prefab data etc
		[ShowIf("@this.allowCreate")]
		public ScriptableObjectRetriever<LandmarkPreset> preset = new ScriptableObjectRetriever<LandmarkPreset>();

		protected override Landmark MakeNew()
		{
			var newLandmark = new Landmark(location.GetTypedFieldValue(), preset.prefabKey);

			return newLandmark;
		}

		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			return location.CalculateField(context);
		}
	}
}