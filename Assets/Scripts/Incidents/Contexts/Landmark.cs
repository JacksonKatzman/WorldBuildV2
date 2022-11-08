using Game.Incidents;
using System;

namespace Game.Incidents
{
	public class Landmark : ILocationAffiliated
	{
		public Location CurrentLocation { get; set; }
	}
}