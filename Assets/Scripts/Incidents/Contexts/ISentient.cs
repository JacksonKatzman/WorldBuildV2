namespace Game.Incidents
{
	public interface ISentient 
	{
		Faction AffiliatedFaction { get; set; }
		public void Die();
	}
}