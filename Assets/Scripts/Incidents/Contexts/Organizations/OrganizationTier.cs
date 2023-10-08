using Game.Enums;
using Game.Generators.Names;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class OrganizationTier : List<OrganizationPosition>
	{
		//whether or not all positions in this tier share titles/responsibilities such as high lords
		private int titlePoints;
		public bool sharedTitle;
		public TitlePair titlePair;
		public OrganizationType organizationType;
		public int tier;

		public OrganizationTier() { }
		public OrganizationTier(Organization org, Faction affiliatedFaction, Race majorityRace, OrganizationType organizationType, int tier, int maxTiers)
		{
			this.organizationType = organizationType;
			this.tier = tier;
			titlePoints = maxTiers - tier;
			//temporary shared title choice - want to make more random in future
			sharedTitle = tier < 2 ? false : true;
			if (sharedTitle)
			{
				titlePair = affiliatedFaction?.namingTheme.GenerateTitle(organizationType, titlePoints);
			}

			//commented out here to allow for adding already created characters as leaders
			//AddPosition(org, affiliatedFaction, majorityRace);
		}

		public OrganizationPosition AddPosition(Organization org, Faction affiliatedFaction, Race majorityRace, Character official = null)
		{
			var position = new OrganizationPosition();
			position.organizationType = organizationType;
			position.titlePair = sharedTitle ? titlePair : affiliatedFaction?.namingTheme.GenerateTitle(organizationType, titlePoints);

			if(official != null)
			{
				position.official = official;
			}
			else if (tier < 2)
			{
				position.SelectNewOfficial(org, affiliatedFaction, majorityRace);
			}

			Add(position);
			return position;
		}
	}
}