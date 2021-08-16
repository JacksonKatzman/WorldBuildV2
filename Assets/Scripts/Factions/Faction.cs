using Game.WorldGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Factions
{
	public class Faction
	{
		private FactionSimulator simulator;

		public string Name => simulator.Name;
		public Color Color => simulator.color;
		public List<City> Cities => simulator.cities;
		public Government Government => simulator.government;
		public List<Tile> Territory => simulator.territory;

		public Faction(FactionSimulator simulator)
		{
			this.simulator = simulator;
		}

		private void BuildLore()
		{
			//Choose some important events that occured in a faction's history and use them to determine:
			//Faction relations, a few cultural quirks
			//Choose some important people from a faction's history and use them to determine:
			//Faction relations, a few cultural quirks

			//Build various lore things:
			//Religion
			//Holidays and cultural events
			//Language/dialects
		}
	}
}