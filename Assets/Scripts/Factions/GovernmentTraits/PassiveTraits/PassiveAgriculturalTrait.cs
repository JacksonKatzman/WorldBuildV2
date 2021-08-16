using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Factions;

[CreateAssetMenu(fileName = nameof(PassiveAgriculturalTrait), menuName = "ScriptableObjects/Government/Traits/Passive/" + nameof(PassiveAgriculturalTrait), order = 1)]
public class PassiveAgriculturalTrait : PassiveGovernmentTrait
{
	public int flatFoodProductionPerWorker = DEFAULT_ADDITIVE_VALUE;
	public int flatMaxFoodByLand = DEFAULT_ADDITIVE_VALUE;

	public float percentFoodProductionPerWorker = DEFAULT_MULITPLICATIVE_PERCENT;
	public float percentSpoilageRate = DEFAULT_MULITPLICATIVE_PERCENT;
	public float percentMaxFoodByLand = DEFAULT_MULITPLICATIVE_PERCENT;

	protected override void UpdateAdditiveValues(FactionSimulator faction)
	{
		faction.foodProductionPerWorker.AddModifier(ModificationType.ADDITIVE, flatFoodProductionPerWorker, 1);
		faction.maxFoodByLand.AddModifier(ModificationType.ADDITIVE, flatMaxFoodByLand, 1);
	}

	protected override void UpdateMultiplicativeValues(FactionSimulator faction)
	{
		faction.foodProductionPerWorker.AddModifier(ModificationType.MULTIPLICATIVE, percentFoodProductionPerWorker /100.0f, 1);
		faction.maxFoodByLand.AddModifier(ModificationType.MULTIPLICATIVE, percentMaxFoodByLand /100.0f, 1);
		faction.spoilageRate.AddModifier(ModificationType.MULTIPLICATIVE, percentSpoilageRate /100.0f, 1);
	}
}
