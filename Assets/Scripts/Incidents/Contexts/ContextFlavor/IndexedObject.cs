namespace Game.Incidents
{
	public class IndexedObject<T>
	{
		public T obj;
		readonly public int index;

		public IndexedObject() { }

		public IndexedObject(int index)
		{
			this.index = index;
		}
	}
}