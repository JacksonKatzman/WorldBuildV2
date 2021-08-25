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
		//public List<Faction> aggressors;

		public Faction originalDefender;
		//public List<Faction> defenders;

		public Dictionary<Faction, int> victories;

		private List<Action> deferredActions;
		private bool resolved = false;

		public War(World world, Faction aggressor, Faction defender)
		{
			this.world = world;

			//aggressors = new List<Faction>();
			//defenders = new List<Faction>();
			deferredActions = new List<Action>();

			//aggressors.Add(aggressor);
			originalAggressor = aggressor;

			//add allies later

			//defenders.Add(defender);
			originalDefender = defender;

			//add allies later

			victories = new Dictionary<Faction, int>();
			victories.Add(originalAggressor, 0);
			victories.Add(originalDefender, 0);
			/*
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
			*/
		}

		public void AdvanceTime()
		{
			if(originalAggressor.cities.Count <= 0 || originalDefender.cities.Count <= 0)
			{
				world.ResolveWar(this);
				resolved = true;
			}
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
			foreach(var pair in victories)
			{
				if(pair.Value >= 5)
				{
					var numTilesTakable = 5;
					for (int i = 0; i < numTilesTakable; i++)
					{
						var possibleTiles = GetAttackableTiles(originalAggressor, originalDefender);
						if (possibleTiles.Count > 0)
						{
							var randomTileIndex = SimRandom.RandomRange(0, possibleTiles.Count);
							var contestedTile = possibleTiles[randomTileIndex];

							AcquireTileByWar(contestedTile, originalAggressor, originalDefender);
						}
						else
						{
							i = numTilesTakable;
						}
					}

					world.ResolveWar(this);
					resolved = true;
					break;
				}
			}
		}

		private void HandleAttackOnTile()
		{
			var random = SimRandom.RandomFloat01();
			var attacker = random < 0.75f ? originalAggressor : originalDefender;
			var defender = attacker == originalAggressor ? originalDefender : originalAggressor;

			var possibleTiles = GetAttackableTiles(attacker, defender);

			if(possibleTiles.Count == 0)
			{
				victories[attacker] = 5;
				return;
			}

			var randomTileIndex = SimRandom.RandomRange(0, possibleTiles.Count);
			var contestedTile = possibleTiles[randomTileIndex];

			var winner = SimulateBattle(attacker, defender, contestedTile);
			victories[winner]++;

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
			contestedTile.ChangeControl(attacker);
		}

		private Faction SimulateBattle(Faction attacker, Faction defender, Tile tile)
		{
			var attackerPower = attacker.population * 0.2f * ((100 - Tile.GetDistanceBetweenTiles(tile, attacker.cities[0].tile))/100.0f);
			var defenderPower = defender.population * 0.2f * ((100 - Tile.GetDistanceBetweenTiles(tile, defender.cities[0].tile)) / 100.0f);
			//ADD more complexity values here based on warlike specialities

			return (attackerPower > defenderPower)? attacker: defender;
		}
	}
}