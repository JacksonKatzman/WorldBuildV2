using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.WorldGeneration;
using Game.Enums;
using Game.Factions;

public class City : Landmark
{
	private static float MAX_BIRTH_RATE = 0.0205f;
	private static float AVERAGE_DEATH_RATE = 0.0078f;
	private static float AVERAGE_FOOD_PRODUCTION = 5.0f;
	private static float AVERAGE_SPOILAGE_RATE = 0.1f;
	private static float MAX_FOOD_BY_LAND = 10000.0f;
	private static float MAX_BURGEONING_TENSION = MAX_FOOD_BY_LAND/10;

	public string name;
	public Tile tile;
	public Faction faction;
    public float food;
    public int population;
	public float burgeoningTension;
	public int burgeoningFactor = 1;

	private float MaximumFoodProduction => MAX_FOOD_BY_LAND * tile.biome.availableLand;

	public City(Tile tile, Faction faction, float food, int population)
	{
		this.tile = tile;
		this.faction = faction;
		this.food = food;
		this.population = population;
		name = NameGenerator.GeneratePersonFirstName(WorldHandler.Instance.PrimaryNameContainer, Gender.NEITHER);
		OutputLogger.LogFormat("{0} City's tile has a land availabilty coef of: {1}", LogSource.IMPORTANT, name, tile.biome.availableLand);
	}

	public override void AdvanceTime()
	{
		HandleFoodGrowth();
		HandleFoodConsumption();
		HandlePopuplationGrowth();
		HandlePopulationDecay();
		HandlePopulationMoves();

		OutputLogger.LogFormat("{0} City now contains a population of: {1}", LogSource.IMPORTANT, name, population.ToString());

		HandleDesertion();
	}

	public void UpdateFaction(Faction faction)
	{
		this.faction = faction;
	}

	private void HandleFoodGrowth()
	{
		float newFood = (AVERAGE_FOOD_PRODUCTION * population * tile.baseFertility) * tile.rainfallValue;
		newFood = Mathf.Clamp(newFood, 0.0f, MaximumFoodProduction);

		OutputLogger.LogFormat("{0} City with pop {1} has fertility {2} and rainfall {3}.", LogSource.CITY, name, population, tile.baseFertility, tile.rainfallValue);
		OutputLogger.LogFormat("{0} City produces {1} new food, bringing food total to {2}", LogSource.CITY, name, newFood, food + newFood);
		food += newFood;
	}

	private void HandleFoodConsumption()
	{
		OutputLogger.LogFormat("{0} City consumes {1} food, bringing food total to {2}", LogSource.CITY, name, population, food - population);
		food -= population;
		var lostFood = food * AVERAGE_SPOILAGE_RATE;
		food -= lostFood;
		OutputLogger.LogFormat("{0} City loses {1} food to natural spoilage, bringing food total to {2}", LogSource.CITY, name, lostFood, food);
	}

	private void HandlePopuplationGrowth()
	{
		var births = 0;
		while((births + population < food) && (births < (MAX_BIRTH_RATE * population)))
		{
			births++;
		}
		population += births;
	}

	private void HandlePopulationDecay()
	{
		population = (int)(population * (1.0f - AVERAGE_DEATH_RATE));
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
			OutputLogger.LogFormat("{0} City's burgeoning tension increased to {1} out of {2}.", LogSource.IMPORTANT, name, burgeoningTension, MAX_BURGEONING_TENSION * (burgeoningFactor));
			OutputLogger.LogFormat("This occured due to a population of {0}, and an available land score of {1}.", LogSource.IMPORTANT, population, tile.biome.availableLand);
			if (burgeoningTension >= MAX_BURGEONING_TENSION * (burgeoningFactor))
			{
				var movingPercentage = WorldHandler.Instance.RandomRange(5, 11) / 100.0f;
				var movingAmount = (int)(population * movingPercentage);
				var foodAmount = movingAmount * 1.3f;
				if (faction.SpawnCityWithinRadius(tile, foodAmount, movingAmount))
				{
					population -= movingAmount;
					food -= foodAmount;
				}
				burgeoningFactor *= 2;
				burgeoningTension = 0.0f;
			}
		}
	}

	private void HandleDesertion()
	{
		if(population <= 0)
		{
			tile.landmarks.Remove(this);
			//should probs destroy it too
		}
	}
}
