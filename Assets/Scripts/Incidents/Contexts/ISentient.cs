using Game.Enums;
using System.Collections.Generic;

namespace Game.Incidents
{
	public interface ISentient 
	{
		Faction AffiliatedFaction { get; set; }
		public List<CharacterTag> CharacterTags { get; set; }
		public void Die();
	}
}