﻿using Game.Debug;
using Game.Factions;
using Game.Terrain;
using System;

namespace Game.Incidents
{
	public class ExpandBordersAction : GenericIncidentAction
	{
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var faction = (context as IFactionAffiliated).AffiliatedFaction;
			var completed = faction.AttemptExpandBorder(1);
			if(completed)
			{
				faction.Influence -= (int)Math.Pow(faction.ControlledTiles, 2);
				if (faction.Influence < 0)
				{
					faction.Influence = 0;
				}

				OutputLogger.Log("Borders expanded!");
			}
		}
	}
}