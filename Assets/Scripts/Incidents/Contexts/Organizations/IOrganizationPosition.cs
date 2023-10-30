namespace Game.Incidents
{
	public interface IOrganizationPosition
	{
		Organization AffiliatedOrganization { get; set; }
		void HandleSuccession();
		string GetTitle(ISentient sentient);
	}
}