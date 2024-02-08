﻿using Game.Simulation;
using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class SimulationOptions : SerializedMonoBehaviour
	{
		public bool DrawFeaturesBeforeSimulation = true;
		public int simulatedYears = 100;
		public int targetFactions = 10;
		public int targetSpecialFactions = 15;
		public int targetCharacters = 120;
		public int targetGreatMonsters = 50;
		[PropertyRange(0.1, 0.7f)]
		public float claimedHexPercentage = 0.3f;
		public Dictionary<FactionPreset, int> factions;
	}
}