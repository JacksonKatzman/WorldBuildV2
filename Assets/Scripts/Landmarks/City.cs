using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.WorldGeneration;
using Game.Enums;
using Game.Factions;
using Game.Generators;

public class City : Landmark
{
	public Tile tile;
    public float food;
    public int population;
	public float burgeoningTension;
	public int burgeoningFactor = 1;

	private float MaximumFoodProduction => faction.Stats.maxFoodByLand.Modified * tile.biome.availableLand;

	public City(Tile tile, Faction faction, float food, int population)
	{
		this.tile = tile;
		this.faction = faction;
		this.food = food;
		this.population = population;

		tile.ChangeControl(faction);

		name = NameGenerator.GeneratePersonFirstName(faction.race, Gender.ANY);
		OutputLogger.LogFormat("{0} City's tile has a land availabilty coef of: {1}", LogSource.IMPORTANT, name, tile.biome.availableLand);
	}

	public override void AdvanceTime()
	{
		//OutputLogger.LogFormat("Beginning {0} City Advance Time!", LogSource.MAIN, name);

		HandleFoodGrowth();
		HandleFoodConsumption();
		HandlePopuplationGrowth();
		HandlePopulationDecay();
		HandlePopulationMoves();

		OutputLogger.LogFormat("{0} City now contains a population of: {1}", LogSource.CITY, name, population.ToString());

		HandleDesertion();

		base.AdvanceTime();
	}

	public void UpdateFaction(Faction faction)
	{
		this.faction = faction;
	}

	private void HandleFoodGrowth()
	{
		//OutputLogger.LogFormat("{0} Faction produces has an average food production per person of: {1}", LogSource.CITY, faction.name, faction.foodProductionPerWorker.modified);
		float newFood = (faction.Stats.foodProductionPerWorker.Modified * population * tile.baseFertility) * tile.rainfallValue;
		newFood = Mathf.Clamp(newFood, 0.0f, MaximumFoodProduction);

		faction.ReportFoodProduced(newFood);

		OutputLogger.LogFormat("{0} City with pop {1} has fertility {2} and rainfall {3}.", LogSource.CITY, name, population, tile.baseFertility, tile.rainfallValue);
		OutputLogger.LogFormat("{0} City produces {1} new food, bringing food total to {2}", LogSource.CITY, name, newFood, food + newFood);
		food += newFood;
	}

	private void HandleFoodConsumption()
	{
		OutputLogger.LogFormat("{0} City consumes {1} food, bringing food total to {2}", LogSource.CITY, name, population, food - population);
		food -= population;
		var lostFood = food * faction.Stats.spoilageRate.Modified;
		food -= lostFood;
		OutputLogger.LogFormat("{0} City loses {1} food to natural spoilage, bringing food total to {2}", LogSource.CITY, name, lostFood, food);
	}

	private void HandlePopuplationGrowth()
	{
		var births = 0;
		while((births + population < food) && (births < (faction.Stats.birthRate.Modified * population)))
		{
			births++;
		}
		population += births;
	}

	private void HandlePopulationDecay()
	{
		population = (int)(population * (1.0f - faction.Stats.deathRate.Modified));
		while(food < 0)
		{
			population--;
			food++;
		}
	}

	private void HandlePopulationMoves()
	{
		var tensionScore = MaximumFoodProduction * 0.85f;
		if (population >= tensionScore)
		{
			burgeoningTension += (population - tensionScore);
			OutputLogger.LogFormat("{0} City's burgeoning tension increased to {1} out of {2}.", LogSource.CITY, name, burgeoningTension, faction.Stats.maxBurgeoningTension.Modified * (burgeoningFactor));
			OutputLogger.LogFormat("This occured due to a population of {0}, and an available land score of {1}.", LogSource.CITY, population, tile.biome.availableLand);
			if (burgeoningTension >= faction.Stats.maxBurgeoningTension.Modified * burgeoningFactor && faction.cities.Count < faction.territory.Count / 5)
			{
				var movingPercentage = SimRandom.RandomRange(5, 11) / 100.0f;
				var movingAmount = (int)(population * movingPercentage);
				var foodAmount = movingAmount * 1.3f;
				if (faction.SpawnCityWithinRadius(tile, foodAmount, movingAmount))
				{
					population -= movingAmount;
					food -= foodAmount;
					burgeoningFactor *= 3;
					burgeoningTension = 0.0f;
				}
			}
		}
	}

	private void HandleDesertion()
	{
		if (population <= 0)
		{
			LandmarkGenerator.DestroyLandmark(this);
		}
	}
}
