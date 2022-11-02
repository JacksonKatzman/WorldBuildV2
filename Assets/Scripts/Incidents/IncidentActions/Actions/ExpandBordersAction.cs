using Game.Factions;
using Game.Terrain;
using System;

namespace Game.Incidents
{
	public class ExpandBordersAction : IncidentAction<Faction>
	{
		public override void PerformAction(IIncidentContext context)
		{
			//Calculate the cell that is best within the ones closest to the current center that arent unclaimed.
			//Can allow for a little wiggle room for more interesting generation
			//Maybe later add some resource or other based criteria for increasing tile weight

			//Once chosen, add those cell coordinates to faction's list of controlled tiles
			//Perhaps perform some confirmation action
			//Send out a TileClaimed context? I guess this is part of the actual incident not this action.
			var faction = context as Faction;
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
/*
		public override void UpdateEditor()
		{
		}
*/
/*
		protected override bool VerifyContextActionFields(IIncidentContext context, Func<int, IIncidentActionField> delayedCalculateAction)
		{
			return true;
		}
*/
	}
}