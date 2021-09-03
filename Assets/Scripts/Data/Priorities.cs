using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;
using System;
using System.Linq;

public class Priorities
{
	public Dictionary<PriorityType, int> priorities;

	public Priorities() : this(10, 10, 10, 10, 10) { }
	public Priorities(int militaryScore, int infrastructureScore, int mercantileScore, int politicalScore, int religiousScore)
	{
		priorities = new Dictionary<PriorityType, int>();
		priorities.Add(PriorityType.MILITARY, Mathf.Clamp(militaryScore, 0, 20));
		priorities.Add(PriorityType.INFRASTRUCTURE, Mathf.Clamp(infrastructureScore, 0, 20));
		priorities.Add(PriorityType.MERCANTILE, Mathf.Clamp(mercantileScore, 0, 20));
		priorities.Add(PriorityType.POLITICAL, Mathf.Clamp(politicalScore, 0, 20));
		priorities.Add(PriorityType.RELIGIOUS, Mathf.Clamp(religiousScore, 0, 20));
	}

	public PriorityType TopPriority()
	{
		return SortedList()[0];
	}

	public List<PriorityType> SortedList()
	{
		Dictionary<int, List<PriorityType>> sorter = new Dictionary<int, List<PriorityType>>();
		foreach(var pair in priorities)
		{
			if(!sorter.ContainsKey(pair.Value))
			{
				sorter.Add(pair.Value, new List<PriorityType>());
			}
			sorter[pair.Value].Add(pair.Key);
		}

		var scoreSortedArray = sorter.OrderByDescending(key => key.Key).ToArray();
		var finalList = new List<PriorityType>();

		for(int index = 0; index < scoreSortedArray.Length; index++)
		{
			while(scoreSortedArray[index].Value.Count > 0)
			{
				var randomIndex = SimRandom.RandomRange(0, scoreSortedArray[index].Value.Count);
				var priorityType = scoreSortedArray[index].Value[randomIndex];
				scoreSortedArray[index].Value.Remove(priorityType);
				finalList.Add(priorityType);
			}
		}

		return finalList;
	}
	public static Priorities operator +(Priorities a, Priorities b)
	{
		var operatedPriorities = new Priorities();
		foreach(PriorityType priorityType in (PriorityType[]) Enum.GetValues(typeof(PriorityType)))
		{
			operatedPriorities.priorities[priorityType] = a.priorities[priorityType] + b.priorities[priorityType];
		}
		return operatedPriorities;
 	}

	public static Priorities operator -(Priorities a, Priorities b)
	{
		var operatedPriorities = new Priorities();
		foreach (PriorityType priorityType in (PriorityType[])Enum.GetValues(typeof(PriorityType)))
		{
			operatedPriorities.priorities[priorityType] = a.priorities[priorityType] - b.priorities[priorityType];
		}
		return operatedPriorities;
	}

	public static Priorities operator /(Priorities a, int b)
	{
		var operatedPriorities = new Priorities();
		foreach (PriorityType priorityType in (PriorityType[])Enum.GetValues(typeof(PriorityType)))
		{
			operatedPriorities.priorities[priorityType] = a.priorities[priorityType] / b;
		}
		return operatedPriorities;
	}

	public int GetTotalScore()
	{
		var score = 0;
		foreach(var pair in priorities)
		{
			score += pair.Value;
		}
		return score;
	}

	public override string ToString()
	{
		return $"MI: {priorities[PriorityType.MILITARY]}" +
			$" / IN: {priorities[PriorityType.INFRASTRUCTURE]}" +
			$" / ME: {priorities[PriorityType.MERCANTILE]}" +
			$" / PO: {priorities[PriorityType.POLITICAL]}" +
			$" / RE: {priorities[PriorityType.RELIGIOUS]}";
	}
}

public class ActionKey
{
	public string fuctionName;
	public int influenceRequirement;
	public Priorities score;

	public ActionKey(string fuctionName, int influenceRequirement, Priorities score)
	{
		this.fuctionName = fuctionName;
		this.influenceRequirement = influenceRequirement;
		this.score = score;
	}
}
