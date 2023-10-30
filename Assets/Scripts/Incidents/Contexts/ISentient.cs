using Game.Enums;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface ISentient : IPermsAffiliated
	{
		Gender Gender { get; set; }
		Faction AffiliatedFaction { get; set; }
		public List<CharacterTag> CharacterTags { get; set; }
		public void Die();
	}
}