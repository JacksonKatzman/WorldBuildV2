namespace Game.Incidents
{
	public interface IOrganizationPosition
	{
		ISentient PrimaryOfficial { get; }
		Organization AffiliatedOrganization { get; set; }
		public int OrganizationTier { get; set; }
		void HandleSuccession(IOrganizationPosition top = null);
		void Update();
		string GetTitle(ISentient sentient);
	}
}