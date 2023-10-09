using Game.Enums;
using Game.Simulation;
using Sirenix.OdinInspector;
using System;

namespace Game.Incidents
{
	public class GetOrCreateLandmarkAction : GetOrCreateAction<Landmark>
	{
		[ShowIf("@this.allowCreate")]
		public InterfacedIncidentActionFieldContainer<ILocationAffiliated> location;

		[ShowIf("@this.allowCreate")]
		public ScriptableObjectRetriever<LandmarkPreset> preset = new ScriptableObjectRetriever<LandmarkPreset>();

		protected override Landmark MakeNew()
		{
			var newLandmark = new Landmark(location.GetTypedFieldValue().CurrentLocation, preset.RetrieveObject());
			if(location.GetTypedFieldValue().GetType() == typeof(City))
			{
				((City)location.GetTypedFieldValue()).Landmarks.Add(newLandmark);
			}

			return newLandmark;
		}

		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			return location.actionField.CalculateField(context);
		}
	}
}