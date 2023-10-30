using Game.Generators.Names;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class OrganizationPositionGrouping : AbstractOrganizationComponent, IOrganizationPosition
	{
		public OrganizationPosition primaryPosition;
		public TitlePair spouseTitles;
		public TitlePair childTitles;
		public List<ISentient> Spouses => GetSpouses(primaryPosition.official);
		public List<ISentient> Children => GetChildren(primaryPosition.official);
		public int maxGroupings;

		public OrganizationPositionGrouping() { }
		public OrganizationPositionGrouping(OrganizationPositionGrouping other)
		{
			primaryPosition = new OrganizationPosition(other.primaryPosition);
			spouseTitles = other.spouseTitles;
			childTitles = other.childTitles;
			maxGroupings = other.maxGroupings;
		}

		public override bool Contains(ISentient sentient, out IOrganizationPosition pos)
		{
			if(primaryPosition.Contains(sentient, out var position))
			{
				pos = this;
				return true;
			}

			else
			{
				pos = null;
				return false;
			}
		}

		public override void GetPositionCount(ref int total, bool careAboutFilled)
		{
			if ((careAboutFilled && primaryPosition.official != null) || !careAboutFilled)
			{
				total += 1;
			}
		}

		public override void GetSentients(ref List<ISentient> sentients)
		{
			primaryPosition.GetSentients(ref sentients);
		}

		public void HandleSuccession()
		{
			foreach(var spouse in Spouses)
			{
				((Character)spouse).OrganizationPosition = null;
			}
			foreach (var child in Children)
			{
				((Character)child).OrganizationPosition = null;
			}

			primaryPosition.HandleSuccession();

			foreach (var spouse in Spouses)
			{
				((Character)spouse).OrganizationPosition = this;
			}
			foreach (var child in Children)
			{
				((Character)child).OrganizationPosition = this;
			}
		}

		public override void Initialize(Organization org)
		{
			AffiliatedOrganization = org;
			primaryPosition.Initialize(org);
		}

		public override bool TryFillNextPosition(out IOrganizationPosition filledPosition)
		{
			if (primaryPosition.official == null)
			{
				HandleSuccession();
				filledPosition = this;
				return true;
			}
			else
			{
				filledPosition = null;
				return false;
			}
		}

		private List<ISentient> GetSpouses(ISentient sentient)
		{
			var spouses = new List<ISentient>();
			if(sentient != null && sentient.GetType() == typeof(Character))
			{
				var character = sentient as Character;
				spouses.AddRange(character.Spouses);
			}

			return spouses;
		}

		private List<ISentient> GetChildren(ISentient sentient)
		{
			var children = new HashSet<ISentient>();
			if (sentient != null && sentient.GetType() == typeof(Character))
			{
				var character = sentient as Character;
				foreach(var child in character.Children)
				{
					children.Add(child);
					foreach (var childSpouse in child.Spouses)
					{
						children.Add(childSpouse);
					}
				}
			}

			return children.ToList();
		}

		public string GetTitle(ISentient sentient)
		{
			if(primaryPosition.official == sentient)
			{
				return primaryPosition.GetTitle(sentient);
			}
			else if(Spouses.Contains(sentient))
			{
				return spouseTitles.GetTitle(sentient.Gender);
			}
			else if(Children.Contains(sentient))
			{
				return childTitles.GetTitle(sentient.Gender);
			}
			else
			{
				return string.Empty;
			}
		}
	}
}