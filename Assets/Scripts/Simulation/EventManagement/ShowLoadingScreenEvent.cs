namespace Game.Simulation
{
	public class ShowLoadingScreenEvent : ISimulationEvent
	{
        public string loadingScreenMainText;

		public ShowLoadingScreenEvent(string loadingScreenMainText)
		{
			this.loadingScreenMainText = loadingScreenMainText;
		}
	}

	public class HideLoadingScreenEvent : ISimulationEvent
	{
		public HideLoadingScreenEvent() { }
	}
}
