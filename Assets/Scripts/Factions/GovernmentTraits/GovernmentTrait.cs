using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Factions
{
	public abstract class GovernmentTrait : ScriptableObject
	{
		public abstract void Invoke(Faction faction);
	}

	public abstract class PassiveGovernmentTrait : GovernmentTrait
	{
		protected abstract void UpdateAdditiveValues(Faction faction);
		protected abstract void UpdateMultiplicativeValues(Faction faction);
		public override void Invoke(Faction faction)
		{
			UpdateAdditiveValues(faction);
			UpdateMultiplicativeValues(faction);
		}
	}

	public abstract class ActiveGovernmentTrait : GovernmentTrait
	{

	}
}