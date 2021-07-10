using Game.Factions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;

[CreateAssetMenu(fileName = nameof(GovernmentType), menuName = "ScriptableObjects/Government/" + nameof(GovernmentType), order = 1)]
public class GovernmentType : ScriptableObject
{
	public List<LeadershipTier> leadershipStructure;
	public List<GovernmentTrait> traits;
	public int influenceRequirement = 10;
}

[System.Serializable]
public class LeadershipStructureNode
{
	public Person occupant;
	public Vector2Int ageRange;
	public Gender requiredGender = Gender.ANY;
	public string title;

	public LeadershipStructureNode(LeadershipStructureNode copyNode)
	{
		occupant = copyNode.occupant;
		ageRange = copyNode.ageRange;
		requiredGender = copyNode.requiredGender;
	}
}

[System.Serializable]
public class LeadershipTier
{
	public List<LeadershipStructureNode> tier;

	public LeadershipTier(LeadershipTier copy)
	{
		tier = new List<LeadershipStructureNode>();
		foreach(LeadershipStructureNode node in copy.tier)
		{
			tier.Add(new LeadershipStructureNode(node));
		}
	}
	
	public LeadershipStructureNode this[int key]
	{
		get
		{
			return tier[key];
		}
		set
		{
			tier[key] = value;
		}
	}

	public IEnumerator<LeadershipStructureNode> GetEnumerator()
	{
		return tier.GetEnumerator();
	}
}
