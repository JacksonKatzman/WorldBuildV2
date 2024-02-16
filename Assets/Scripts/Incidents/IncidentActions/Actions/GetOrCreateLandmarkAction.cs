using Game.Enums;
using Game.Simulation;
using Sirenix.OdinInspector;
using System;
using System.Linq;

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

			return newLandmark;
		}

		protected override void Complete()
		{
			if(madeNew)
			{
				var loc = location.GetTypedFieldValue().CurrentLocation;
				var factions = ContextDictionaryProvider.GetCurrentContexts<Faction>();
				foreach(var faction in factions)
				{
					if(faction.ControlledTileIndices.Contains(loc.TileIndex))
					{
						actionField.GetTypedFieldValue().AffiliatedFaction = faction;
						break;
					}
				}
			}
			base.Complete();
		}

		protected override bool VersionSpecificVerify(IIncidentContext context)
		{
			return location.actionField.CalculateField(context);
		}
	}
}