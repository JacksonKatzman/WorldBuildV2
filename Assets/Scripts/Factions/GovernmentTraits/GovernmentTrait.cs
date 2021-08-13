using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Factions
{
	public abstract class GovernmentTrait : ScriptableObject
	{
		public abstract void Invoke(FactionSimulator faction);
	}

	public abstract class PassiveGovernmentTrait : GovernmentTrait
	{
		public static int DEFAULT_ADDITIVE_VALUE = 0;
		public static float DEFAULT_MULITPLICATIVE_PERCENT = 100;
		protected abstract void UpdateAdditiveValues(FactionSimulator faction);
		protected abstract void UpdateMultiplicativeValues(FactionSimulator faction);
		public override void Invoke(FactionSimulator faction)
		{
			UpdateAdditiveValues(faction);
			UpdateMultiplicativeValues(faction);
		}
	}

	public abstract class ActiveGovernmentTrait : GovernmentTrait
	{

	}
}