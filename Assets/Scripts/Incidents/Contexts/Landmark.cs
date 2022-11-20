using Game.Generators.Items;
using Game.Incidents;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class Landmark : InertIncidentContext, ILocationAffiliated, IInventoryAffiliated
	{
		public Location CurrentLocation { get; set; }

		public override Type ContextType => typeof(Landmark);

		public List<Item> Inventory { get; set; }

		public Landmark() { }
		public Landmark(Location location)
		{
			CurrentLocation = location;
			Inventory = new List<Item>();
		}
	}
}