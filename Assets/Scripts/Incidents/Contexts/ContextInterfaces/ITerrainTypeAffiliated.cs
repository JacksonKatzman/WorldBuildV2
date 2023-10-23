using Game.Terrain;

namespace Game.Incidents
{
	public interface ITerrainTypeAffiliated
	{
		BiomeTerrainType AffiliatedTerrainType { get; }
	}
}