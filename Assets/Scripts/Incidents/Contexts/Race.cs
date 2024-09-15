using Game.Simulation;
using System;

namespace Game.Incidents
{
	public class Race : InertIncidentContext, IRaceAffiliated
	{
		public override Type ContextType => typeof(Race);
		public Race AffiliatedRace => this;
		public int MinAge { get; set; }
		public int MaxAge { get; set; }

        public override string Description => $"RACE DESCRIPTION";

        public RacePreset racePreset;

		public Race() { }

		public Race(RacePreset racePreset)
		{
			this.racePreset = racePreset;
			MinAge = racePreset.minAge;
			MaxAge = racePreset.maxAge;
			Name = racePreset.name;
		}

		//make preset for races
		//use that as part of constructor
		//use constructors when making world
		//pass info from race to faction to make naming theme for faction
	}
}