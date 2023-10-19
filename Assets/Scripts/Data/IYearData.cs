namespace Game.Data
{
	public interface IYearData { }

	public class YearData<T> : IYearData
	{
		public int year;
		public T data;

		public YearData(int year, T data)
		{
			this.year = year;
			this.data = data;
		}
	}
}