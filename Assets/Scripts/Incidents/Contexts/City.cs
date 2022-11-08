using Game.Generators.Items;
using Game.Incidents;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class City : InertIncidentContext, IFactionAffiliated, ILocationAffiliated
	{
		public override Type ContextType => typeof(City);
		public Location CurrentLocation { get; set; }
		public Faction AffiliatedFaction { get; set; }
		public int Population { get; set; }
		public int Wealth { get; set; }
		public List<Resource> Resources { get; set; }
		public List<Landmark> Landmarks { get; set; }

		public City(Faction faction, Location location, int population, int wealth)
		{
			AffiliatedFaction = faction;
			CurrentLocation = location;
			Population = population;
			Wealth = wealth;
		}
	}
}