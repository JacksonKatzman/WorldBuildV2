using Game.Factions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(PassiveAdministrativeTrait), menuName = "ScriptableObjects/Government/Traits/Passive/" + nameof(PassiveAdministrativeTrait), order = 1)]
public class PassiveAdministrativeTrait : PassiveGovernmentTrait
{
	public int flatMaxBurgeoningTension = DEFAULT_ADDITIVE_VALUE;

	public float percentBirthRate = DEFAULT_MULITPLICATIVE_PERCENT;
	public float percentDeathRate = DEFAULT_MULITPLICATIVE_PERCENT;
	public float percentMaxBurgeoningTension = DEFAULT_MULITPLICATIVE_PERCENT;
	protected override void UpdateAdditiveValues(FactionSimulator faction)
	{
		faction.maxBurgeoningTension.modified += flatMaxBurgeoningTension;
	}

	protected override void UpdateMultiplicativeValues(FactionSimulator faction)
	{
		faction.birthRate.modified *= (percentBirthRate/100.0f);
		faction.deathRate.modified *= (percentDeathRate/100.0f);
		faction.maxBurgeoningTension.modified *= (percentMaxBurgeoningTension/100.0f);
	}
}
