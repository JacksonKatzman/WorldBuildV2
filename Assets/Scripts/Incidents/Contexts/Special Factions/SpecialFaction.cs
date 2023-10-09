using Game.Enums;
using Game.Utilities;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	abstract public class SpecialFaction : Faction
	{
		private static List<SpecialFactionBiasContainer> FactionTypes = new List<SpecialFactionBiasContainer>
		{
			{ new SpecialFactionBiasContainer(12, 10, 0, 0, typeof(MagicAcademy)) }
		};

		override public int Population
		{
			get
			{
				return (int)populationFloat;
			}
			set
			{
				populationFloat = (float)value;
			}
		}

		public ILocationAffiliated Location { get; set; }

		public override int ControlledTiles => 1;
		public override int NumCities => 0;

		private float populationFloat;

		public static Type CalculateFactionType(int political, int economic, int religious, int military)
		{
			var container = FactionTypes[0];
			var previousBest = CalculateScores(container, political, economic, religious, military);
			for (int i = 1; i < FactionTypes.Count; i++)
			{
				var total = CalculateScores(FactionTypes[i], political, economic, religious, military);
				if(total < previousBest)
				{
					container = FactionTypes[i];
				}
			}
			return container.factionType;
		}

		public void SetLocation(ILocationAffiliated location)
		{
			Location = location;
			//also use this to set the tile they control to the location
		}

		public SpecialFaction()
		{
			FactionRelations = new Dictionary<IIncidentContext, int>();
			Cities = new List<City>();
			FactionsAtWarWith = new List<IIncidentContext>();

			Priorities = new Dictionary<OrganizationType, int>();
			Priorities[OrganizationType.POLITICAL] = SimRandom.RandomRange(1, 4);
			Priorities[OrganizationType.ECONOMIC] = SimRandom.RandomRange(1, 4);
			Priorities[OrganizationType.RELIGIOUS] = SimRandom.RandomRange(1, 4);
			Priorities[OrganizationType.MILITARY] = SimRandom.RandomRange(1, 4);
		}

		override public void UpdateContext()
		{
			NumIncidents = 0;
		}

		private static int CalculateScores(SpecialFactionBiasContainer container, int political, int economic, int religious, int military)
		{
			return CalculateScore(political, container.politicalBias) +
					CalculateScore(economic, container.economicBias) +
					CalculateScore(religious, container.religiousBias) +
					CalculateScore(military, container.militaryBias);
		}

		private static int CalculateScore(int input, int fromDictionary)
		{
			var difference = fromDictionary - input;
			if(difference > 0)
			{
				difference *= 3;
			}
			else if(difference < 0)
			{
				difference = Math.Abs(difference);
			}
			return difference;
		}
	}

	public struct SpecialFactionBiasContainer
	{
		public int politicalBias;
		public int economicBias;
		public int religiousBias;
		public int militaryBias;
		public Type factionType;

		public SpecialFactionBiasContainer(int politicalBias, int economicBias, int religiousBias, int militaryBias, Type factionType)
		{
			this.politicalBias = politicalBias;
			this.economicBias = economicBias;
			this.religiousBias = religiousBias;
			this.militaryBias = militaryBias;
			this.factionType = factionType;
		}
	}
}