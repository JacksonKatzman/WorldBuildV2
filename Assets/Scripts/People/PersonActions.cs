using System.Collections;
using UnityEngine;
using Game.Data.EventHandling;

namespace Game.People
{
	public abstract class PersonActions
	{
		public static void TestPersonAction(Person person)
		{
			EventManager.Instance.Dispatch(new BaseRPEvent(person, "Base Person Event"));
		}

		public static void Misc_DrunkenBrawl(Person person)
		{

		}

		public static void Misc_PurchaseExoticPet(Person person)
		{

		}

		public static void Misc_UnfortunateFall(Person person)
		{

		}

		public static void Magic_DiscoverSpell(Person person)
		{

		}

		public static void Lore_DiscoverProphecy(Person person)
		{

		}

		public static void Lore_JoinCult(Person person)
		{

		}
	}

	public abstract class LeaderActions
	{
		public static void TestLeaderAction(Person person)
		{
			EventManager.Instance.Dispatch(new BaseRPEvent(person, "Base Leader Event"));
		}

		public static void HireAssassin_Scam(Person person)
		{
			//Assassin's true intentions found out, punished
			var optionA = 25 + 2 * person.stats.wisdomMod;
			//Assassin gets away
			var optionB = 25 - person.stats.luckMod * 2;
			//Assassin gets away and sells your secret to another faction
			var optionC = 25 - person.stats.luckMod;

			var chance = SimRandom.RandomRange(0, optionA + optionB + optionC);
			var result = string.Format("{0} hires an assassin for a hefty fee, and charges them to take care of a troubling local lord who's rise to prominence has been far too swift. The assassin takes the money and flees, ", person.Name);

			if((chance -= optionA) <= 0)
			{
				result += string.Format("but {0} saw through their deception, and the theif was quickly apprehended.", person.Name);
				person.influence += 20;
			}
			if ((chance -= optionB) <= 0)
			{
				result += string.Format("disappearing into the night.");
				person.influence -= 50;
				var thief = PersonGenerator.GeneratePerson(null, new Vector2Int(16, 40), Enums.Gender.ANY, 150);
				thief.stats.agility = SimRandom.RandomRange(15, 20);
				EventManager.Instance.Dispatch(new BaseRPEvent(thief, string.Format("Posing as an assassin for hire, {0} stole {1} gold coins from {2} before making a stealthy escape.", thief.Name, SimRandom.RandomRange(1300, 4500), person.Name)));
			}
			else
			{
				result += string.Format("sure to sell what they know to anyone who will listen.");
				person.influence -= 100;
				var thief = PersonGenerator.GeneratePerson(null, new Vector2Int(16, 40), Enums.Gender.ANY, 150);
				thief.stats.agility = SimRandom.RandomRange(15, 20);
				thief.stats.charisma = SimRandom.RandomRange(15, 20);
				EventManager.Instance.Dispatch(new BaseRPEvent(thief, string.Format("Posing as an assassin for hire, {0} stole {1} gold coins from {2} before making a stealthy escape, with a further secret to sell.", thief.Name, SimRandom.RandomRange(1300, 4500), person.Name)));
			}

			EventManager.Instance.Dispatch(new BaseRPEvent(person, result));
		}

		public static void HireAssassin_RandomRival(Person person)
		{
			var roll = SimRandom.RollXDY(1, 20, 0) + person.stats.charismaMod * 2;
			var randomFaction = person.faction;
			while(randomFaction == person.faction && SimulationManager.Instance.World.factions.Count > 1)
			{
				var randomIndex = SimRandom.RandomRange(0, SimulationManager.Instance.World.factions.Count);
				randomFaction = SimulationManager.Instance.World.factions[randomIndex];
			}

			var randomTier = randomFaction.government.leadershipStructure[SimRandom.RandomRange(0, randomFaction.government.leadershipStructure.Count)].tier;

			var randomTarget = randomTier[SimRandom.RandomRange(0, randomTier.Count)].occupant;

			var result = string.Format("{0} hires an assassin to kill {1}, a governing member of the {2} faction. ", person.Name, randomTarget.Name, randomFaction.name);

			if (roll > 20)
			{
				PersonGenerator.HandleDeath(randomTarget, "Assassination");
				person.influence += 100;
				result += string.Format("Assassination succeeded, was discovered, but was spun to {0}'s political advantage.", person.Name);
			}
			else if (roll > 16)
			{
				PersonGenerator.HandleDeath(randomTarget, "Assassination");
				result += string.Format("Assassination succeeded, nobody knows who sent them.");
			}
			else if (roll > 12)
			{
				PersonGenerator.HandleDeath(randomTarget, "Assassination");
				person.influence -= 100;
				randomFaction.ModifyFactionTension(person.faction, 500);
				result += string.Format("Assassination succeeded, was discovered, and reflected poorly upon {0}.", person.Name);
			}
			else if (roll > 9)
			{
				var alternateFaction = person.faction;
				while ((alternateFaction == person.faction || alternateFaction == randomFaction) && SimulationManager.Instance.World.factions.Count > 2)
				{
					var randomIndex = SimRandom.RandomRange(0, SimulationManager.Instance.World.factions.Count);
					alternateFaction = SimulationManager.Instance.World.factions[randomIndex];
				}

				randomFaction.ModifyFactionTension(alternateFaction, 500);
				result += string.Format("Assassination fails, but when questioned named a member of {0} as their employer.", alternateFaction.name);
			}
			else if (roll > 6)
			{
				result += string.Format("Assassination failed, but nobody knows who sent them.");
			}
			else
			{
				person.influence -= 120;
				randomFaction.ModifyFactionTension(person.faction, 500);
				result += string.Format("Assassination fails, was discovered, and reflected poorly upon {0}.", person.Name);
			}

			EventManager.Instance.Dispatch(new BaseRPEvent(person, result));
		}

		public static void HireAssassin_DoubleCross(Person person)
		{
			var roll = SimRandom.RollXDY(1, 20, 0) + person.stats.wisdomMod;
			var result = string.Format("{0} hires an assassin to kill a rival, but the assassin double crosses them. ", person.Name);

			if(roll > 15)
			{
				result += string.Format("{0} discovers the double cross just in time, and has the assassin put to death.", person.Name);
				person.influence += 40;
			}
			else
			{
				result += string.Format("{0} is slain by the assassin.", person.Name);
				PersonGenerator.HandleDeath(person, "Assassination");
			}

			EventManager.Instance.Dispatch(new BaseRPEvent(person, result));
		}

		public static void HireAssassin_Catastrophe(Person person)
		{
			//trigger a world event based on the outcome of assassinate rival?
		}

		public static void HireAssassin_Relative(Person person)
		{
			//wait to implement until family trees are important
		}

		public static void HireAssassin_Dignatary(Person person)
		{
			//do later? possibly superfluous would be for lore
		}

		public static void HireAdventurers_CriminalJustice(Person person)
		{

		}

		public static void HireAdventurers_TreasureHunt(Person person)
		{

		}

		public static void HireAdventurers_KillQuest(Person person)
		{

		}

		public static void HireAdventurers_Exploration(Person person)
		{

		}

		public static void HireAdventurers_CaravanGuards(Person person)
		{

		}

		public static void Mercantile_NegotiateTradeDeal(Person person)
		{

		}

		public static void Mercantile_SurpriseTaxation(Person person)
		{

		}

		public static void Mercantile_DiscoverResource(Person person)
		{

		}

		public static void Mercantile_InstituteTariff(Person person)
		{

		}

		public static void SitInJudgement_CriminalLord(Person person)
		{

		}

		public static void SitInJudgement_HighProfileMurder(Person person)
		{

		}

		public static void SitInJudgement_GuildFued(Person person)
		{

		}

		public static void Intermediary_VassalTradeDeal(Person person)
		{

		}

		public static void RideOut_FightInfestation(Person person)
		{

		}

		public static void RideOut_FightHorde(Person person)
		{

		}

		public static void RideOut_FightMonster(Person person)
		{

		}
		public static void PoliticalManeuver_ArrangeCoup(Person person)
		{

		}

		public static void PoliticalManeuver_DiscreditRival(Person person)
		{

		}

		public static void PoliticalManeuver_CourtAddition(Person person)
		{

		}

		public static void PoliticalManeuver_PeaceTalks(Person person)
		{

		}

		public static void PoliticalManeuver_CastOutExile(Person person)
		{

		}

		public static void ComissionArt_Weapon(Person person)
		{

		}

		public static void ComissionArt_Armor(Person person)
		{

		}

		public static void ComissionArt_Trinket(Person person)
		{

		}

		public static void ComissionArt_Statue(Person person)
		{

		}

		public static void ComissionArt_Other(Person person)
		{

		}

		public static void Misc_ThrowFeast(Person person)
		{

		}

		public static void Misc_Infidelity(Person person)
		{

		}

		public static void Lore_DeclareSacred(Person person)
		{

		}
	}
}