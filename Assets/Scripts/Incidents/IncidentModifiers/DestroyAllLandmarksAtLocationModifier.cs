using Game.Generators;
using Game.WorldGeneration;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class DestroyAllLandmarksAtLocationModifier : IncidentModifier, ITileLocationContainer
	{
		private List<Tile> locations;

		public List<Tile> Locations => locations;

		public DestroyAllLandmarksAtLocationModifier(List<IIncidentTag> tags, float probability) : base(tags, probability)
		{
		}

		public override void Run(IncidentContext context)
		{
			base.Run(context);

			foreach (var location in locations)
			{
				location.landmarks.ForEach(x => LandmarkGenerator.DestroyLandmark(x));
			}
		}
	}
}