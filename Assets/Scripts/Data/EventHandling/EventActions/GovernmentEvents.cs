using Game.Enums;
using Game.Factions;
using System.Collections;
using UnityEngine;

namespace Game.Data.EventHandling
{
	public abstract class GovernmentEvents
	{
		public static void AddLeaderToTier_10(Government government)
		{
			if(government.leadershipStructure.Count == 1)
			{
				government.BuildNewGovernmentTier();
			}
			else
			{
				government.AddLeaderToRandomTierWeighted();
			}
		}

		public static void AddLeadershipTier_10(Government government)
		{
			government.BuildNewGovernmentTier();
		}

		public static void ModifyStyle_10(Government government)
		{
			if(government.priorities.GetTotalScore() == 0)
			{
				var randomPriority = (PriorityType)SimRandom.RandomRange(0, 5);
				government.priorities.priorities[randomPriority] += 3;
			}
			else
			{
				var topPriority = government.priorities.SortedList()[0];
				government.priorities.priorities[topPriority] += 1;
				var randomPriority = (PriorityType)SimRandom.RandomRange(0, 5);
				government.priorities.priorities[randomPriority] += 1;
			}
		}

		public static void AddPassiveTrait_00(Government government)
		{

		}

		public static void AddActiveTrait_00(Government government)
		{

		}

		public static void AddHoliday_10(Government government)
		{
			government.holidays.Add(new Holiday.Holiday());
		}

		public static void ChangePolicy_00(Government government)
		{

		}
	}
}