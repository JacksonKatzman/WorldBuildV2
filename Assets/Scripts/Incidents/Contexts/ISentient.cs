using Game.Enums;
using Game.Generators.Names;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface ISentient : IPermsAffiliated
	{
		IIncidentContext Context { get; }
		Gender Gender { get; set; }
		Faction AffiliatedFaction { get; set; }
		CharacterName CharacterName { get; set; }
		public Organization AffiliatedOrganization { get; }
		public IOrganizationPosition OrganizationPosition { get; }
		public List<CharacterTrait> CharacterTraits { get; set; }
		public void Die();
	}
}