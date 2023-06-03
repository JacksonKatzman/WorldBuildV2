﻿using Game.Enums;
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
		public Race Race { get; set; }
		public Faction AffiliatedFaction { get; set; }
		public OrganizationPosition OfficialPosition { get; set; }
		void Die();
	}

	public static class ICharacterExtensions
	{
		public static bool CheckDestroyed(this ICharacter person)
		{
			var cuspA = person.Race.MaxAge * 0.3f;
			var cuspB = person.Race.MaxAge * 0.85f;
			var deathChance = -Mathf.Atan(((cuspA + (cuspB - cuspA)) - person.Age) / (Mathf.Sqrt(cuspB - cuspA) * Mathf.PI / 2.0f)) / Mathf.PI + 0.5f;

			var randomValue = SimRandom.RandomFloat01();
			return randomValue <= deathChance;
		}
	}
}