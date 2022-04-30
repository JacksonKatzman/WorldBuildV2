using Game.Enums;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public interface IIncidentTag
	{
		public bool CompareTag(IncidentContext context);
	}

	[System.Serializable]
	public class PriorityTag : IIncidentTag
	{
		public PriorityType priorityType;

		public PriorityTag(PriorityType priorityType)
		{
			this.priorityType = priorityType;
		}

		public bool CompareTag(IncidentContext context)
		{
			//return priorityType == context.priorities.TopPriority();
			foreach(var tag in context.tags)
			{
				if((tag as PriorityTag)?.priorityType == priorityType)
				{
					return true;
				}
			}
			return false;
		}
	}

	[System.Serializable]
	public class SpecialCaseTag : IIncidentTag
	{
		public SpecialCaseTagType tagType;

		public SpecialCaseTag(SpecialCaseTagType tagType)
		{
			this.tagType = tagType;
		}

		public bool CompareTag(IncidentContext context)
		{
			foreach (var tag in context.tags)
			{
				if (tag is SpecialCaseTag specialCaseTag)
				{
					if (specialCaseTag.tagType == tagType)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	[System.Serializable]
	public class WorldTag : IIncidentTag
	{
		public List<WorldTagType> tags;

		public WorldTag(List<WorldTagType> tags)
		{
			this.tags = tags;
		}

		public bool CompareTag(IncidentContext context)
		{
			//return tags.All(x => context.worldTags.Contains(x));
			foreach (var tag in context.tags)
			{
				if (tag is WorldTag worldTag)
				{
					var matches = tags.All(x => worldTag.tags.Contains(x));
					if(matches)
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	[System.Serializable]
	public class InstigatorTag : IIncidentTag
	{
		[ValueDropdown("GrabInterfaceTypes")]
		public Type type;

		public InstigatorTag(Type type)
		{
			this.type = type;
		}

		public bool CompareTag(IncidentContext context)
		{
			//return context.instigator.GetType() == type;
			foreach (var tag in context.tags)
			{
				if (tag is InstigatorTag instigatorTag)
				{
					if (instigatorTag.type == type)
					{
						return true;
					}
				}
			}
			return false;
		}

		private IEnumerable<Type> GrabInterfaceTypes()
		{
			return IncidentHelpers.GetInterfaceTypes(typeof(IIncidentInstigator));
		}
	}
}