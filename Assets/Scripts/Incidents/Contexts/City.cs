﻿using Game.Generators.Items;
using Game.Simulation;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class City : InertIncidentContext, IFactionAffiliated, ILocationAffiliated, IInventoryAffiliated
	{
		public override Type ContextType => typeof(City);
		public Location CurrentLocation { get; set; }
		public Faction AffiliatedFaction { get; set; }
		virtual public int Population
		{
			get
			{
				return (int)populationFloat;
			}
			set
			{
				populationFloat = (float)value;
			}
		}
		public int Wealth { get; set; }
		public int NumItems => Inventory.Items.Count;
		public bool IsOnBorder => SimulationUtilities.FindBorderWithinFaction(AffiliatedFaction).Contains(CurrentLocation.TileIndex);
		public List<Resource> Resources { get; set; }
		public List<Landmark> Landmarks { get; set; }
		public List<MinorCharacter> MinorCharacters { get; set; }

		public Inventory Inventory { get; set; }
		private float populationFloat;

		public City(Faction faction, Location location, int population, int wealth)
		{
			AffiliatedFaction = faction;
			CurrentLocation = location;
			Population = population;
			Wealth = wealth;
			Inventory = new Inventory();
		}

		public int GenerateWealth()
		{
			return 1;
		}

		public void UpdatePopulation()
		{
			populationFloat *= 1.011f;
		}

		public void GenerateMinorCharacters(int amount)
		{
			for(var i = 0; i < amount; i++)
			{
				var character = new MinorCharacter(AffiliatedFaction);
				SimulationManager.Instance.world.AddContextImmediate(character);
			}
		}
	}
}