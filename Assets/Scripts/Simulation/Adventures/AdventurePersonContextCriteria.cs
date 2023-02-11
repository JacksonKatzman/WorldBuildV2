using Game.Incidents;

namespace Game.Simulation
{
	public class AdventurePersonContextCriteria : AdventureContextCriteria<Person>
	{
		//public GetOrCreatePersonAction GetOrCreatePerson;
		public override void RetrieveContext()
		{
			//temporary
			Context = new Person();
		}
	}
}
