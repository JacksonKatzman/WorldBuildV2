using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Factions;

public abstract class FactionActions
{
	public static void DoNothing(Faction faction)
	{
		OutputLogger.LogFormat("{0} Faction did nothing interesting during this cycle.", Game.Enums.LogSource.FACTIONACTION, faction.name);
	}

	public static void ExpandTerritoryToFreeTile(Faction faction)
	{
		faction.ExpandTerritory();
	}

	public static void GenerateInfluence(Faction faction)
	{
		//Replace this with an actual calculation based on faction factors
		faction.influence++;
		OutputLogger.LogFormat("{0} Faction used political dealings to increase it's influence.", Game.Enums.LogSource.FACTIONACTION, faction.name);
	}

	public static void RecruitTroops(Faction faction)
	{
		faction.EventRecruitTroops();
	}
}
