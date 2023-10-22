using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Incidents
{
	public class HandleOpposedForcesOutcomeAction : GenericIncidentAction
	{
		private static Dictionary<string, Func<Faction, Faction, Faction>> conflictFunctions = new Dictionary<string, Func<Faction, Faction, Faction>>()
		{
			{"POLITICAL", OpposedPoliticalConflict }, {"ECONOMIC", OpposedEconomicConflict },
			{"RELIGIOUS", OpposedReligiousConflict }, {"MILITARY", OpposedMilitaryConflict }
		};

		public InterfacedIncidentActionFieldContainer<IFactionAffiliated> attacker;
		public InterfacedIncidentActionFieldContainer<IFactionAffiliated> defender;

		[ValueDropdown("GetConflictTypes")]
		public string conflictType;

		[HideInInspector]
		public ContextualIncidentActionField<Faction> winner = new ContextualIncidentActionField<Faction>();
		[HideInInspector]
		public ContextualIncidentActionField<Faction> loser = new ContextualIncidentActionField<Faction>();

		[ReadOnly, ShowInInspector]
		public string WinnerResultID => winner?.ActionFieldIDString;
		[ReadOnly, ShowInInspector]
		public string LoserResultID => loser?.ActionFieldIDString;

		public override bool VerifyAction(IIncidentContext context)
		{
			winner.AllowNull = true;
			var baseVerified = base.VerifyAction(context);
			if (string.IsNullOrEmpty(conflictType) || !baseVerified)
			{
				return false;
			}

			var attackingFaction = (Faction)attacker.GetTypedFieldValue();
			var defendingFaction = (Faction)defender.GetTypedFieldValue();
			var winningFaction = conflictFunctions[conflictType](attackingFaction, defendingFaction);
			winner.value = winningFaction;
			loser.value = winningFaction == attackingFaction ? defendingFaction : attackingFaction;
			return true;
		}
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{

		}

		private static Faction OpposedPoliticalConflict(Faction attacker, Faction defender)
		{
			return attacker.PoliticalPriority > defender.PoliticalPriority ? attacker : defender;
		}

		private static Faction OpposedEconomicConflict(Faction attacker, Faction defender)
		{
			return attacker.EconomicPriority > defender.EconomicPriority ? attacker : defender;
		}

		private static Faction OpposedReligiousConflict(Faction attacker, Faction defender)
		{
			return attacker.ReligiousPriority > defender.ReligiousPriority ? attacker : defender;
		}
		private static Faction OpposedMilitaryConflict(Faction attacker, Faction defender)
		{
			return attacker.MilitaryPower > defender.MilitaryPower ? attacker : defender;
		}

		private IEnumerable<string> GetConflictTypes()
		{
			return conflictFunctions.Keys;
		}
	}
}