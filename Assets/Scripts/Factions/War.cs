using Game.WorldGeneration;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;
using System;

namespace Game.Factions
{
	public class War: ITimeSensitive
	{
		private World world;

		public Faction originalAggressor;
		public List<Faction> aggressors;

		public Faction originalDefender;
		public List<Faction> defenders;

		private List<Action> deferredActions;
		private bool resolved = false;

		public War(World world, Faction aggressor, Faction defender)
		{
			this.world = world;

			aggressors = new List<Faction>();
			defenders = new List<Faction>();
			deferredActions = new List<Action>();

			aggressors.Add(aggressor);
			originalAggressor = aggressor;
			aggressor.RecruitPercentOfPopulation(SimRandom.RandomRange(15, 30));
			//add allies later

			defenders.Add(defender);
			originalDefender = defender;
			defender.RecruitPercentOfPopulation(SimRandom.RandomRange(15, 30));
			//add allies later

			foreach (Faction agg in aggressors)
			{
				foreach(Faction def in defenders)
				{
					agg.SetFactionTension(def, float.MinValue);
				}
			}

			foreach (Faction def in aggressors)
			{
				foreach (Faction agg in defenders)
				{
					def.SetFactionTension(agg, float.MinValue);
				}
			}
		}

		public void AdvanceTime()
		{
			HandleWarResolution();
			//for now one faction will just attack a tile and they will fight over it
			if (!resolved)
			{
				HandleAttackOnTile();
				HandleWarResolution();
				HandleDeferredActions();
			}
		}

		private void HandleDeferredActions()
		{
			foreach (Action action in deferredActions)
			{
				action.Invoke();
			}
			deferredActions.Clear();
		}

		private void HandleWarResolution()
		{
			foreach(Faction faction in defenders)
			{
				if(faction.military.Count <= 0)
				{
					foreach(Faction aggressor in aggressors)
					{
						aggressor.SetFactionTension(faction, SimRandom.RandomRange(-1000,0));
						deferredActions.Add(() => { defenders.Remove(faction); });
					}
				}
			}

			foreach (Faction faction in aggressors)
			{
				if (faction.military.Count <= 0)
				{
					foreach (Faction defender in defenders)
					{
						defender.SetFactionTension(faction, SimRandom.RandomRange(-1000, 0));
						deferredActions.Add(() => { aggressors.Remove(faction); });
					}
				}
			}

			if(aggressors.Count == 0 && defenders.Count == 0)
			{
				//draw
				world.ResolveWar(this);
				resolved = true;
			}
			else if(defenders.Count == 0)
			{
				//aggressor wins
				var numTilesTaken = Mathf.Min(2 + originalAggressor.military.Count/100, originalDefender.territory.Count);
				for(int i = 0; i < numTilesTaken; i++)
				{
					var possibleTiles = originalDefender.territory;

					var randomTileIndex = SimRandom.RandomRange(0, possibleTiles.Count);
					var contestedTile = possibleTiles[randomTileIndex];

					AcquireTileByWar(contestedTile, originalAggressor, originalDefender);
				}
				world.ResolveWar(this);
				resolved = true;
			}
			else if(aggressors.Count == 0)
			{
				//defender wins
				var numTilesTaken = Mathf.Min(2 + originalAggressor.territory.Count/3, originalAggressor.territory.Count);
				for (int i = 0; i < numTilesTaken; i++)
				{
					var possibleTiles = originalAggressor.territory;

					var randomTileIndex = SimRandom.RandomRange(0, possibleTiles.Count);
					var contestedTile = possibleTiles[randomTileIndex];

					AcquireTileByWar(contestedTile, originalDefender, originalAggressor);
				}

				world.ResolveWar(this);
				resolved = true;
			}
		}

		private void HandleAttackOnTile()
		{
			var random = SimRandom.RandomFloat01();
			var attacker = random < 0.75f ? aggressors[0] : defenders[0];
			var defender = random < 0.75f ? defenders[0] : aggressors[0];

			var possibleTiles = GetAttackableTiles(attacker, defender);

			if(possibleTiles.Count == 0)
			{
				defenders.Remove(defender);
				return;
			}

			var randomTileIndex = SimRandom.RandomRange(0, possibleTiles.Count);
			var contestedTile = possibleTiles[randomTileIndex];

			var winner = SimulateBattle(attacker, defender);

			if(winner == attacker)
			{
				AcquireTileByWar(contestedTile, attacker, defender);
			}
		}

		private List<Tile> GetAttackableTiles(Faction attacker, Faction defender)
		{
			var possibleTiles = new List<Tile>();
			foreach (Tile tile in attacker.GetBorderTiles())
			{
				if (defender.territory.Contains(tile))
				{
					possibleTiles.Add(tile);
				}
			}
			return possibleTiles;
		}

		private void AcquireTileByWar(Tile contestedTile, Faction attacker, Faction defender)
		{
			defender.territory.Remove(contestedTile);
			attacker.territory.Add(contestedTile);

			foreach (Landmark landmark in contestedTile.landmarks)
			{
				if (landmark is City city)
				{
					defender.RemoveCity(city);
					attacker.AddCity(city);
				}
			}
		}

		private Faction SimulateBattle(Faction attacker, Faction defender)
		{
			//Archers fire first, starting with defender
			SimulateArrowFire(defender, attacker);
			SimulateArrowFire(attacker, defender);

			SimulateCavalryCharge(attacker, defender);
			SimulateCavalryCharge(defender, attacker);

			SimulateFlanking(attacker, defender);

			SimulateInfantryBattle(attacker, defender);

			var winner = attacker.military.Count > defender.military.Count ? attacker : defender;
			return winner;
		}

		private void SimulateArrowFire(Faction att, Faction def)
		{
			var attacker = att.military;
			var defender = def.military;
			var arrows = attacker.troops[TroopType.ARCHER];

			while(arrows > 0)
			{
				if (defender.troops[TroopType.HEAVY_CAVALRY] > 0 && arrows > 3)
				{
					defender.troops[TroopType.HEAVY_CAVALRY]--;
					arrows -= 3;
				}
				else if (defender.troops[TroopType.HEAVY_INFANTRY] > 0 && arrows > 2)
				{
					defender.troops[TroopType.HEAVY_INFANTRY]--;
					arrows -= 2;
				}
				else if (defender.troops[TroopType.LIGHT_INFANTRY] > 0)
				{
					defender.troops[TroopType.LIGHT_INFANTRY]--;
					arrows--;
				}
				else if (defender.troops[TroopType.LIGHT_CAVALRY] > 0 && arrows > 2)
				{
					defender.troops[TroopType.LIGHT_CAVALRY]--;
					arrows -= 2;
				}
				else
				{
					arrows = 0;
				}
			}
		}

		private void SimulateCavalryCharge(Faction att, Faction def)
		{
			var attacker = att.military;
			var defender = def.military;
			var heavyCav = attacker.troops[TroopType.HEAVY_CAVALRY];
			var heavyInf = defender.troops[TroopType.HEAVY_INFANTRY];
			var lightInf = defender.troops[TroopType.LIGHT_INFANTRY];

			var heavyCavCasualties = (int)((heavyInf * 0.2f) * (SimRandom.RandomRange(60,101)/100));
			var heavyInfCasualties = (int)((heavyCav * 0.35f) * (SimRandom.RandomRange(60, 101) / 100));

			attacker.troops[TroopType.HEAVY_CAVALRY] = Mathf.Clamp(heavyCav - heavyCavCasualties, 0, heavyCav);
			defender.troops[TroopType.HEAVY_INFANTRY] = Mathf.Clamp(heavyInf - heavyInfCasualties, 0, heavyInf);

			if(attacker.troops[TroopType.HEAVY_CAVALRY] > 0 && defender.troops[TroopType.HEAVY_INFANTRY] == 0)
			{
				defender.troops[TroopType.LIGHT_INFANTRY] = Mathf.Clamp(lightInf - attacker.troops[TroopType.HEAVY_CAVALRY], 0, lightInf);
			}
			if (attacker.troops[TroopType.HEAVY_CAVALRY] > 0 && defender.troops[TroopType.LIGHT_INFANTRY] == 0)
			{
				defender.troops[TroopType.ARCHER] = Mathf.Clamp(defender.troops[TroopType.ARCHER] - attacker.troops[TroopType.HEAVY_CAVALRY], 0, defender.troops[TroopType.ARCHER]);
			}
			//LIGHT CAV
		}

		private void SimulateFlanking(Faction att, Faction def)
		{
			var attacker = att.military;
			var defender = def.military;
			var attLightCav = attacker.troops[TroopType.LIGHT_CAVALRY];
			var attArchers = attacker.troops[TroopType.ARCHER];
			var defLightCav = defender.troops[TroopType.LIGHT_CAVALRY];
			var defArchers = defender.troops[TroopType.ARCHER];

			var attLightCavCasualties = (int)((defLightCav * 0.4f) * (SimRandom.RandomRange(60, 101) / 100));
			var defLightCavCasualties = (int)((attLightCav * 0.4f) * (SimRandom.RandomRange(60, 101) / 100));

			attacker.troops[TroopType.LIGHT_CAVALRY] = Mathf.Clamp(attLightCav - attLightCavCasualties, 0, attLightCav);
			defender.troops[TroopType.LIGHT_CAVALRY] = Mathf.Clamp(defLightCav - defLightCavCasualties, 0, defLightCav);

			if(attacker.troops[TroopType.LIGHT_CAVALRY] > 0)
			{
				defender.troops[TroopType.ARCHER] = Mathf.Clamp(defArchers - attacker.troops[TroopType.LIGHT_CAVALRY], 0, defArchers);
			}
			if (defender.troops[TroopType.LIGHT_CAVALRY] > 0)
			{
				attacker.troops[TroopType.ARCHER] = Mathf.Clamp(attArchers - attacker.troops[TroopType.LIGHT_CAVALRY], 0, attArchers);
			}
		}

		private void SimulateInfantryBattle(Faction att, Faction def)
		{
			var attacker = att.military;
			var defender = def.military;

			var attHeavyInf = attacker.troops[TroopType.HEAVY_INFANTRY];
			var attLightInf = attacker.troops[TroopType.LIGHT_INFANTRY];

			var defHeavyInf = defender.troops[TroopType.HEAVY_INFANTRY];
			var defLightInf = defender.troops[TroopType.LIGHT_INFANTRY];

			var attHeavyInfCasualties = (int)((defHeavyInf * 0.2f + defLightInf * 0.35f) * (SimRandom.RandomRange(60, 101) / 100));
			var attLightInfCasualties = (int)((defHeavyInf * 0.1f + defLightInf * 0.2f) * (SimRandom.RandomRange(60, 101) / 100));

			var defHeavyInfCasualties = (int)((attHeavyInf * 0.2f + attLightInf * 0.35f) * (SimRandom.RandomRange(60, 101) / 100));
			var defLightInfCasualties = (int)((attHeavyInf * 0.1f + attLightInf * 0.2f) * (SimRandom.RandomRange(60, 101) / 100));

			attacker.troops[TroopType.HEAVY_INFANTRY] = Mathf.Clamp(attHeavyInf - attHeavyInfCasualties, 0, attHeavyInf);
			attacker.troops[TroopType.LIGHT_INFANTRY] = Mathf.Clamp(attLightInf - attLightInfCasualties, 0, attLightInf);

			defender.troops[TroopType.HEAVY_INFANTRY] = Mathf.Clamp(defHeavyInf - defHeavyInfCasualties, 0, defHeavyInf);
			defender.troops[TroopType.LIGHT_INFANTRY] = Mathf.Clamp(defLightInf - defLightInfCasualties, 0, defLightInf);

			attHeavyInf = attacker.troops[TroopType.HEAVY_INFANTRY];
			attLightInf = attacker.troops[TroopType.LIGHT_INFANTRY];
			var attLightCav = attacker.troops[TroopType.LIGHT_CAVALRY];

			defHeavyInf = defender.troops[TroopType.HEAVY_INFANTRY];
			defLightInf = defender.troops[TroopType.LIGHT_INFANTRY];
			var defLightCav = defender.troops[TroopType.LIGHT_CAVALRY];

			attacker.troops[TroopType.LIGHT_CAVALRY] = Mathf.Clamp(attLightCav - (defHeavyInf + defLightInf) / 2, 0, attLightCav);
			defender.troops[TroopType.LIGHT_CAVALRY] = Mathf.Clamp(defLightCav - (attHeavyInf + attLightInf) / 2, 0, defLightCav);
		}
	}
}