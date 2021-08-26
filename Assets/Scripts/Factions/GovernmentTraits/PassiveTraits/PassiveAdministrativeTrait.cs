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
	protected override void UpdateAdditiveValues(Faction faction)
	{
		faction.maxBurgeoningTension.AddModifier(ModificationType.ADDITIVE, flatMaxBurgeoningTension, 1);
	}

	protected override void UpdateMultiplicativeValues(Faction faction)
	{
		faction.birthRate.AddModifier(ModificationType.MULTIPLICATIVE, percentBirthRate/100.0f, 1);
		faction.deathRate.AddModifier(ModificationType.MULTIPLICATIVE, percentDeathRate /100.0f, 1);
		faction.maxBurgeoningTension.AddModifier(ModificationType.MULTIPLICATIVE, percentMaxBurgeoningTension /100.0f, 1);
	}
}
