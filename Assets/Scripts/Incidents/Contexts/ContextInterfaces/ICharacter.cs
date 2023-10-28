using Game.Enums;
using Game.Generators.Names;
using Game.Utilities;
using UnityEngine;

namespace Game.Incidents
{
	public interface ICharacter 
	{
		public string Name { get; }
		public CharacterName CharacterName { get; set; }
		public int Age { get; set; }
		public Gender Gender { get; set; }
		public Race AffiliatedRace { get; set; }
		public Faction AffiliatedFaction { get; set; }
		public Organization AffiliatedOrganization { get; set; }
		public OrganizationPosition OfficialPosition { get; }
		void Die();
	}

	public static class ICharacterExtensions
	{
		public static bool CheckDestroyed(this ICharacter person)
		{
			
			float deathChance = 1.0f;
			var cuspA = person.AffiliatedRace.MaxAge * 0.60f;
			var cuspB = person.AffiliatedRace.MaxAge * 0.95f;
			if (person.Age < cuspA)
			{
				/*
				var a = 2f / 3f * cuspA;
				var b = cuspB - a;
				var c = a + b - person.Age;
				var d = Mathf.Sqrt(b) /16f;
				var e = c / d;
				var f = -Mathf.Atan(e);
				var g = f / Mathf.PI + 0.5f;
				*/

				deathChance = -Mathf.Atan(((2f / 3f * cuspA + (cuspB - (2f / 3f * cuspA))) - person.Age) / (Mathf.Sqrt(cuspB - (2f / 3f * cuspA)) / 16f)) / Mathf.PI + 0.5f;
			}
			else
			{
				deathChance = -Mathf.Atan(((cuspA + (cuspB - cuspA)) - person.Age) / (Mathf.Sqrt(cuspB - cuspA) * Mathf.PI / 16f)) / Mathf.PI + 0.5f;
			}

			var randomValue = SimRandom.RandomFloat01();
			return randomValue <= deathChance;
		}
	}
}