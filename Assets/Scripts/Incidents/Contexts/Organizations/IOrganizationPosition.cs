namespace Game.Incidents
{
	public interface IOrganizationPosition
	{
		Organization AffiliatedOrganization { get; set; }
		void HandleSuccession();
		void Update();
		string GetTitle(ISentient sentient);
	}
}