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
	public Vector2Int ageRange;
	public Gender requiredGender = Gender.ANY;
}

[System.Serializable]
public class LeadershipTier
{
	public List<LeadershipStructureNode> tier;
	
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
