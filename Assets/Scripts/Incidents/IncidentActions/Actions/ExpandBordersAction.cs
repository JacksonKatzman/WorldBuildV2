using Game.Debug;
using Game.Factions;
using Game.Terrain;
using System;

namespace Game.Incidents
{
	public class ExpandBordersAction : GenericIncidentAction
	{
		public int numberOfTimes;
		public int influenceCostPerExpansion;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			var faction = (context as IFactionAffiliated).AffiliatedFaction;
			var completed = faction.AttemptExpandBorder(numberOfTimes);
			if(completed)
			{
				if(influenceCostPerExpansion > 0)
				{
					faction.Influence -= (numberOfTimes * influenceCostPerExpansion);
				}
				if (faction.Influence < 0)
				{
					faction.Influence = 0;
				}

				OutputLogger.Log("Borders expanded!");
			}
		}
	}
}