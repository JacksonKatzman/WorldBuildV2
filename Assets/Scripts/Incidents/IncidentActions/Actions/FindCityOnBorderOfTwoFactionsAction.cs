using Game.Simulation;
using Game.Utilities;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public class FindCityOnBorderOfTwoFactionsAction : GenericIncidentAction
	{
		public InterfacedIncidentActionFieldContainer<IFactionAffiliated> attacker;
		public InterfacedIncidentActionFieldContainer<IFactionAffiliated> defender;

		[HideInInspector]
		public ContextualIncidentActionField<City> borderCity = new ContextualIncidentActionField<City>();

		[ReadOnly, ShowInInspector]
		public string CityResultID => borderCity?.ActionFieldIDString;

		public override bool VerifyAction(IIncidentContext context)
		{
			borderCity.AllowNull = true;
			var baseVerified = base.VerifyAction(context);
			if (!baseVerified)
			{
				return false;
			}

			var attackingFaction = (Faction)attacker.GetTypedFieldValue();
			var defendingFaction = (Faction)defender.GetTypedFieldValue();
			var attackerOuterBorder = SimulationUtilities.FindBorderOutsideFaction(attackingFaction);
			var defendersCities = defendingFaction.Cities;

			var possibilities = defendersCities.Where(x => attackerOuterBorder.Contains(x.CurrentLocation.TileIndex)).ToList();
			if(possibilities == null || possibilities.Count == 0)
			{
				return false;
			}

			borderCity.value = SimRandom.RandomEntryFromList(possibilities);
			
			return true;
		}
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{

		}
	}
}