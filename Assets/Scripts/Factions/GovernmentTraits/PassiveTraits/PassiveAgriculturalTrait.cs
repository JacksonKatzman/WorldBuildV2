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
		faction.foodProductionPerWorker.modified += (float)flatFoodProductionPerWorker;
		faction.maxFoodByLand.modified += flatMaxFoodByLand;
	}

	protected override void UpdateMultiplicativeValues(FactionSimulator faction)
	{
		faction.foodProductionPerWorker.modified *= (percentFoodProductionPerWorker/100.0f);
		faction.maxFoodByLand.modified *= (percentMaxFoodByLand/100.0f);
		faction.spoilageRate.modified *= (percentSpoilageRate/100.0f);
	}
}
