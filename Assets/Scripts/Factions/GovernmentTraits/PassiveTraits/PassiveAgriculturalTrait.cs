using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Factions;

[CreateAssetMenu(fileName = nameof(PassiveAgriculturalTrait), menuName = "ScriptableObjects/Government/Traits/Passive/" + nameof(PassiveAgriculturalTrait), order = 1)]
public class PassiveAgriculturalTrait : PassiveGovernmentTrait
{
	public float flatFoodProductionPerWorker = 0;
	public float flatMaxFoodByLand = 0;

	public float percentFoodProductionPerWorker = 100.0f;
	public float percentSpoilageRate = 100.0f;
	public float percentMaxFoodByLand = 100.0f;

	protected override void UpdateAdditiveValues(Faction faction)
	{
		faction.foodProductionPerWorker.modified += flatFoodProductionPerWorker;
		faction.maxFoodByLand.modified += flatMaxFoodByLand;
	}

	protected override void UpdateMultiplicativeValues(Faction faction)
	{
		faction.foodProductionPerWorker.modified *= percentFoodProductionPerWorker;
		faction.maxFoodByLand.modified *= percentMaxFoodByLand;
		faction.spoilageRate.modified *= percentSpoilageRate;
	}
}
