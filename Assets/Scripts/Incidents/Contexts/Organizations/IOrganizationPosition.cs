namespace Game.Incidents
{
	public interface IOrganizationPosition
	{
		Organization AffiliatedOrganization { get; set; }
		void HandleSuccession(IOrganizationPosition top = null);
		void Update();
		string GetTitle(ISentient sentient);
	}
}