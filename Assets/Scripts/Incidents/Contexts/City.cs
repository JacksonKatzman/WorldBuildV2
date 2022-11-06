using Game.Generators.Items;
using Game.Incidents;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class City : InertIncidentContext
	{
		public override Type ContextType => typeof(City);
		public Faction Faction { get; set; }
		public int Population { get; set; }
		public int Wealth { get; set; }
		public List<Resource> Resources { get; set; }
		public List<Landmark> Landmarks { get; set; }

		public City(Faction faction, int population, int wealth)
		{
			Faction = faction;
			Population = population;
			Wealth = wealth;
		}
	}
}