using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Factions
{
	public class Government
	{
		//Person leader;
		GovernmentType governmentType;

		public Government(GovernmentType type)
		{
			governmentType = type;
		}

		public void UpdateFactionUsingPassiveTraits(Faction faction)
		{
			foreach(GovernmentTrait trait in governmentType.traits)
			{
				if(trait is PassiveGovernmentTrait)
				{
					trait.Invoke(faction);
				}
			}
		}
	}
}