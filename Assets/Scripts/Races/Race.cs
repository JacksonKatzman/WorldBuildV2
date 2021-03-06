using Game.Creatures;
using Game.Factions;
using System.Collections;
using UnityEngine;

namespace Game.Races
{
	[CreateAssetMenu(fileName = nameof(Race), menuName = "ScriptableObjects/Races/" + nameof(Race), order = 1)]
	public class Race : ScriptableObject
	{
		public int appearanceWeight;
		public SerializableStatBlock stats;
		public FactionStats culturalStats;

		public NameFormat nameFormat;
	}
}