using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;
using Game.Data;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System;
using Games.Visuals;
using System.Linq;

namespace Game.WorldGeneration
{
	[CreateAssetMenu(fileName = nameof(OldBiome), menuName = "ScriptableObjects/Biomes/" + nameof(OldBiome), order = 1)]
	public class OldBiome : SerializedScriptableObject
	{
		public string biomeType;
		public LandType landType;
		public LandTemperature temperature;

		[Range(0.0f, 1.0f)]
		public float rainfallThreshold;
		[Range(0.0f, 1.0f)]
		public float fertilityThreshold;
		[Range(0.0f, 1.0f)]
		public float availableLand;

		[SerializeField]
		List<PropContainer> propContainers;

		public Dictionary<int, List<GameObject>> propDictionary => BuildPropDictionary();

		public static OldBiome GetRandomBiomeByLandType(List<OldBiome> biomes, LandType landType)
		{
			var matches = biomes.Where(x => x.landType == landType).ToList();
			return SimRandom.RandomEntryFromList(matches);
		}

		public static OldBiome CalculateBiomeType(List<OldBiome> biomes, LandType landType, float rainfall, float fertility)
		{
			return null;
		}

		private Dictionary<int, List<GameObject>> BuildPropDictionary()
		{
			var dictionary = new Dictionary<int, List<GameObject>>();
			foreach(var container in propContainers)
			{
				dictionary.Add(container.weight, container.props);
			}

			return dictionary;
		}
	}
}
