using Game.Enums;

namespace Game.Incidents
{
	public interface IAlignmentAffiliated
	{
		public OrganizationType PriorityAlignment { get; }
		public int LawfulChaoticAlignmentAxis { get; set; }
		public int GoodEvilAlignmentAxis { get; set; }
	}
}