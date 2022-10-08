namespace Game.Incidents
{
	public interface IIncidentContextProvider<T> where T : IIncidentContext
	{
		T GetContext();
		void UpdateContext();
		void DeployContext();
	}
}