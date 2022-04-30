using System.Collections.Generic;

namespace Game.Incidents
{
	public class RandomTileModifier : IncidentModifier
	{
		public RandomTileModifier(List<IIncidentTag> tags, float probability) : base(tags, probability) { }

		public override void Setup()
		{
			base.Setup();

			var tile = SimulationManager.Instance.World.GetRandomTile();
			ProvideModifierInfo(x => (x as ITileLocationContainer)?.Locations.Add(tile));
		}
	}
}