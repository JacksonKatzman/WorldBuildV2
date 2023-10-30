namespace Game.Incidents
{
	public interface IPermsAffiliated
	{
		int PoliticalPriority { get; set; }
		int EconomicPriority { get; set; }
		int ReligiousPriority { get; set; }
		int MilitaryPriority { get; set; }
	}
}