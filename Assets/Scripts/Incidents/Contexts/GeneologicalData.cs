namespace Game.Incidents
{
	public class GeneologicalData
	{
		public int verticallyRemoved;
		public int siblingallyRemoved;
		public int spousallyRemoved;
		public int finalValue;
		public int iterations;

		public GeneologicalData(int verticallyRemoved, int siblingallyRemoved, int spousallyRemoved, int finalValue, int iterations)
		{
			this.verticallyRemoved = verticallyRemoved;
			this.spousallyRemoved = spousallyRemoved;
			this.siblingallyRemoved = siblingallyRemoved;
			this.finalValue = finalValue;
			this.iterations = iterations;
		}
	}
}