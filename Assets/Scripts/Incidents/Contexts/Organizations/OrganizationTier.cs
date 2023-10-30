using Sirenix.OdinInspector;
using System.Collections.Generic;

namespace Game.Incidents
{
	[HideReferenceObjectPicker]
	public class OrganizationTier
	{
		public List<AbstractOrganizationComponent> components = new List<AbstractOrganizationComponent>();

		public OrganizationTier() { }

		public OrganizationTier(OrganizationTier other)
		{
			components = new List<AbstractOrganizationComponent>();
			foreach(var component in other.components)
			{
				if(component.GetType() == typeof(SubOrganization))
				{
					var subOrg = component as SubOrganization;
					components.Add(new SubOrganization(subOrg));
				}
				else if(component.GetType() == typeof(OrganizationPosition))
				{
					var template = component as OrganizationPosition;
					for (int x = 0; x < template.maxPositions; x++)
					{
						components.Add(new OrganizationPosition(template));
					}
				}
				else if (component.GetType() == typeof(OrganizationPositionGrouping))
				{
					var template = component as OrganizationPositionGrouping;
					for (int x = 0; x < template.maxGroupings; x++)
					{
						components.Add(new OrganizationPositionGrouping(template));
					}
				}
			}
		}

		public void Initialize(Organization org)
		{
			foreach(var component in components)
			{
				component.Initialize(org);
			}
		}

		public bool Contains(ISentient sentient, out IOrganizationPosition position)
		{
			foreach (var component in components)
			{
				if (component.Contains(sentient, out position))
				{
					return true;
				}
			}

			position = null;
			return false;
		}

		public bool TryFillNextPosition(out IOrganizationPosition filledPosition)
		{
			foreach(var component in components)
			{
				if(component.TryFillNextPosition(out filledPosition))
				{
					return true;
				}
			}

			filledPosition = null;
			return false;
		}

		public void GetSentients(ref List<ISentient> sentients)
		{
			foreach (var component in components)
			{
				component.GetSentients(ref sentients);
			}
		}

		public void GetPositionCount(ref int total, bool careAboutFilled)
		{
			foreach (var component in components)
			{
				component.GetPositionCount(ref total, careAboutFilled);
			}
		}
	}
}