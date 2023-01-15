using Game.Simulation;
using System;

namespace Game.Incidents
{
	public class Race : InertIncidentContext
	{
		public override Type ContextType => typeof(Race);
		public int MinAge { get; set; }
		public int MaxAge { get; set; }
		public RacePreset racePreset;

		public Race(RacePreset racePreset)
		{
			this.racePreset = racePreset;
			MinAge = racePreset.minAge;
			MaxAge = racePreset.maxAge;
		}

		//make preset for races
		//use that as part of constructor
		//use constructors when making world
		//pass info from race to faction to make naming theme for faction
	}
}