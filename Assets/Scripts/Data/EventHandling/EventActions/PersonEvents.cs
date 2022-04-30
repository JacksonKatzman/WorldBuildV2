using System.Collections;
using UnityEngine;
using Game.Data.EventHandling;
using Game.Enums;
using System.Collections.Generic;
using Game.Data.EventHandling.EventRecording;

namespace Game.Data.EventHandling
{
	public abstract class PersonEvents
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

	public abstract class LeaderActions_Military
	{
		public static void RideOut_FightInfestation_01(Person person)
		{
			var record = new EventRecord();

			var creature = DataManager.Instance.GetRandomCreature(false, true, CreatureType.BEAST);
			var numPoints = SimRandom.RandomRange(20, 100);
			var pointValue = (int)creature.type + 1;
			var numCreatures = numPoints / pointValue;

			record.AddContext("{0} rides out with their retainers to fight off an infestation of " + numCreatures + " rabid {1}'s.", person, creature);

			var roll = SimRandom.RollXDY(1, 20) + person.stats.GetModValue(StatType.STRENGTH);

			if(roll > 20)
			{
				//spawn holiday
				var holiday = new Holiday.Holiday();
				person.faction.government.holidays.Add(holiday);
				record.AddContext("The foe is swept away with ease, and the locals declare the day {0}, in celebration of this great victory.", holiday);
			}
			else if(roll > 6)
			{
				//normal victory
				record.AddContext("After an unexpectedly difficult fight, {0} emerges bloodied but victorious.", person);
			}
			else
			{
				//defeat and death
				PersonGenerator.HandleDeath(person, "Killed in battle");
				record.AddContext("{0} waded into the infestation, and was never heard from again.", person);
			}
			SimulationManager.Instance.eventRecorder.eventRecords.Add(record);
		}

		public static void RideOut_FightHorde_01(Person person)
		{
			var record = EventCatalyst.MonsterGathering(SimRandom.RandomEntryFromList(person.faction.cities), CreatureType.HUMANOID);

			record.AddContext("{0} rides out with their retainers to defend the realm against the rampaging horde.", person);

			var roll = SimRandom.RollXDY(1, 20) + person.stats.GetModValue(StatType.STRENGTH);

			if (roll > 20)
			{
				//spawn holiday
				var holiday = new Holiday.Holiday();
				person.faction.government.holidays.Add(holiday);
				record.AddContext("The foe is swept away with ease, and the locals declare the day {0}, in celebration of this great victory.", holiday);
			}
			else if (roll > 9)
			{
				//normal victory
				record.AddContext("After an unexpectedly difficult fight, {0} emerges bloodied but victorious.", person);
			}
			else
			{
				//defeat and death
				PersonGenerator.HandleDeath(person, "Killed in battle");
				record.AddContext("{0} and all of their followers were overrun by the horde.", person);
			}
		}

		public static void RideOut_FightMonster_01(Person person)
		{

		}

		public static void ComissionArt_Weapon_01(Person person)
		{

		}

		public static void ComissionArt_Armor_01(Person person)
		{

		}
	}

	public abstract class LeaderActions_Infrastructure
	{
		public static void HireAdventurers_KillQuest_01(Person person)
		{

		}

		public static void HireAdventurers_Exploration_01(Person person)
		{

		}
	}

	public abstract class LeaderActions_Mercantile
	{
		public static void HireAdventurers_TreasureHunt_01(Person person)
		{

		}

		public static void HireAdventurers_CaravanGuards_01(Person person)
		{

		}

		public static void Mercantile_NegotiateTradeDeal_01(Person person)
		{

		}

		public static void Mercantile_SurpriseTaxation_01(Person person)
		{

		}

		public static void Mercantile_DiscoverResource_01(Person person)
		{

		}

		public static void Mercantile_InstituteTariff_01(Person person)
		{

		}

		public static void ComissionArt_Trinket_01(Person person)
		{

		}

		public static void ComissionArt_Statue_01(Person person)
		{

		}

		public static void ComissionArt_Other_01(Person person)
		{

		}
	}

	public abstract class LeaderActions_Political
	{
		public static void HireAssassin_Scam_01(Person person)
		{
			//Assassin's true intentions found out, punished
			var optionA = 25 + 2 * person.stats.GetModValue(StatType.WISDOM);
			//Assassin gets away
			var optionB = 25 - person.stats.GetModValue(StatType.LUCK) * 2;
			//Assassin gets away and sells your secret to another faction
			var optionC = 25 - person.stats.GetModValue(StatType.LUCK);

			var chance = SimRandom.RandomRange(0, optionA + optionB + optionC);
			var result = string.Format("{0} hires an assassin for a hefty fee, and charges them to take care of a troubling local lord who's rise to prominence has been far too swift. The assassin takes the money and flees, ", person.Name);

			if ((chance -= optionA) <= 0)
			{
				result += string.Format("but {0} saw through their deception, and the theif was quickly apprehended.", person.Name);
				person.influence += 20;
			}
			if ((chance -= optionB) <= 0)
			{
				result += string.Format("disappearing into the night.");
				person.influence -= 50;
				var thief = new Person(null, SimRandom.RandomRange(16, 40), Enums.Gender.ANY, 150, new List<RoleType> { RoleType.ROGUE });

				thief.stats.stats[StatType.DEXTERITY] = SimRandom.RandomRange(15, 20);
				PersonGenerator.RegisterPerson(thief);
				EventManager.Instance.Dispatch(new BaseRPEvent(thief, string.Format("Posing as an assassin for hire, {0} stole {1} gold coins from {2} before making a stealthy escape.", thief.Name, SimRandom.RandomRange(1300, 4500), person.Name)));
			}
			else
			{
				result += string.Format("sure to sell what they know to anyone who will listen.");
				person.influence -= 100;
				var thief = new Person(null, SimRandom.RandomRange(16, 40), Enums.Gender.ANY, 150, new List<RoleType> { RoleType.ROGUE });

				thief.stats.stats[StatType.DEXTERITY] = SimRandom.RandomRange(15, 20);
				thief.stats.stats[StatType.CHARISMA] = SimRandom.RandomRange(15, 20);
				PersonGenerator.RegisterPerson(thief);
				EventManager.Instance.Dispatch(new BaseRPEvent(thief, string.Format("Posing as an assassin for hire, {0} stole {1} gold coins from {2} before making a stealthy escape, with a further secret to sell.", thief.Name, SimRandom.RandomRange(1300, 4500), person.Name)));
			}

			EventManager.Instance.Dispatch(new BaseRPEvent(person, result));
		}

		public static void HireAssassin_RandomRival_01(Person person)
		{
			var roll = SimRandom.RollXDY(1, 20, 0) + person.stats.GetModValue(StatType.CHARISMA) * 2;
			var randomFaction = person.faction;
			while (randomFaction == person.faction && SimulationManager.Instance.World.factions.Count > 1)
			{
				var randomIndex = SimRandom.RandomRange(0, SimulationManager.Instance.World.factions.Count);
				randomFaction = SimulationManager.Instance.World.factions[randomIndex];
			}

			var randomTarget = randomFaction.People[SimRandom.RandomRange(0, randomFaction.People.Count)];

			var result = string.Format("{0} hires an assassin to kill {1}, a governing member of the {2} faction. ", person.Name, randomTarget.Name, randomFaction.Name);

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
				result += string.Format("Assassination fails, but when questioned named a member of {0} as their employer.", alternateFaction.Name);
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

		public static void HireAssassin_DoubleCross_01(Person person)
		{
			var roll = SimRandom.RollXDY(1, 20, 0) + person.stats.GetModValue(StatType.WISDOM);
			var result = string.Format("{0} hires an assassin to kill a rival, but the assassin double crosses them. ", person.Name);

			if (roll > 15)
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

		public static void HireAssassin_Catastrophe_01(Person person)
		{
			//trigger a world event based on the outcome of assassinate rival?
		}

		public static void HireAssassin_Relative_01(Person person)
		{
			//wait to implement until family trees are important
		}

		public static void HireAssassin_Dignatary_01(Person person)
		{
			//do later? possibly superfluous would be for lore
		}

		public static void HireAdventurers_CriminalJustice_01(Person person)
		{

		}

		public static void SitInJudgement_CriminalLord_01(Person person)
		{

		}

		public static void SitInJudgement_HighProfileMurder_01(Person person)
		{

		}

		public static void SitInJudgement_GuildFued_01(Person person)
		{

		}

		public static void Intermediary_VassalTradeDeal_01(Person person)
		{

		}

		public static void PoliticalManeuver_ArrangeCoup_01(Person person)
		{

		}

		public static void PoliticalManeuver_DiscreditRival_01(Person person)
		{

		}

		public static void PoliticalManeuver_CourtAddition_01(Person person)
		{

		}

		public static void PoliticalManeuver_PeaceTalks_01(Person person)
		{

		}

		public static void PoliticalManeuver_CastOutExile_01(Person person)
		{

		}
	}

	public abstract class LeaderActions_Religious
	{
		public static void TestLeaderAction_01(Person person)
		{
			EventManager.Instance.Dispatch(new BaseRPEvent(person, "Base Leader Event"));
		}

		public static void Lore_DeclareSacred_01(Person person)
		{

		}
	}

	public abstract class LeaderActions_Misc
	{
		public static void Misc_Infidelity_01(Person person)
		{

		}

		public static void Misc_ThrowFeast_01(Person person)
		{

		}
	}
}