using Game.Enums;
using Game.Simulation;
using Sirenix.OdinInspector;
using System;

namespace Game.Incidents
{
	public class GetOrCreateLandmarkAction : GetOrCreateAction<Landmark>
	{
		[ShowIf("@this.allowCreate")]
		public LocationActionField location;

		[ShowIf("@this.allowCreate")]
		public LandmarkType landmarkType;

		//**I think i got confused and made two systems for this. Will not use this one for now, but might need to in future.**

		//need a scriptable object type for LandmarkStyle that acts as a type and includes prefab data etc
		//[ShowIf("@this.allowCreate")]
		//public ScriptableObjectRetriever<LandmarkPreset> preset = new ScriptableObjectRetriever<LandmarkPreset>();

		protected override Landmark MakeNew()
		{
			var newLandmark = new Landmark(location.GetTypedFieldValue(), landmarkType);

			return newLandmark;
		}

		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			return location.CalculateField(context);
		}
	}
}