﻿using System;

namespace Game.Incidents
{
	public class Race : InertIncidentContext
	{
		public override Type ContextType => typeof(Race);
		public int UpperAgeLimit => 110;
	}
}