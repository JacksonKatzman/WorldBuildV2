using Game.Enums;
using Game.Generators.Names;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface ISentient : IPermsAffiliated
	{
		Gender Gender { get; set; }
		Faction AffiliatedFaction { get; set; }
		CharacterName CharacterName { get; set; }
		public Organization AffiliatedOrganization { get; }
		public IOrganizationPosition OrganizationPosition { get; }
		public List<CharacterTag> CharacterTags { get; set; }
		public void Die();
	}
}