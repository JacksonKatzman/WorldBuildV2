using Game.Incidents;
using System;

namespace Game.Incidents
{
	public class Landmark : InertIncidentContext, ILocationAffiliated
	{
		public Location CurrentLocation { get; set; }

		public override Type ContextType => typeof(Landmark);

		public Landmark() { }
		public Landmark(Location location)
		{
			CurrentLocation = location;
		}
	}
}