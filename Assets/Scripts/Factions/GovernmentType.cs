using Game.Factions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;

[CreateAssetMenu(fileName = nameof(GovernmentType), menuName = "ScriptableObjects/Government/" + nameof(GovernmentType), order = 1)]
public class GovernmentType : ScriptableObject
{
	public List<LeadershipTier> leadershipStructure;
	public int influenceRequirement = 10;

	public GovernmentType(OldFaction faction)
	{
		GenerateRandomGovernment(faction);
	}

	private void GenerateRandomGovernment(OldFaction faction)
	{
		//need a way to handle deciding on number of leaders and their titles
		//will only generate for the first tier for the entire simulation for now
		//will make a separate method to fill in the rest of the structure once factions finalize
		//have titles be the same for now, build score based title generator later
		var node = new LeadershipStructureNode(new Vector2Int(18, 68), Gender.ANY);

		leadershipStructure = new List<LeadershipTier>();
		leadershipStructure.Add(new LeadershipTier());
		leadershipStructure[0].tier.Add(node);
	
	}
}

[System.Serializable]
public class LeadershipStructureNode
{
	public Vector2Int ageRange;
	public Gender requiredGender = Gender.ANY;
	public Dictionary<Gender, string> genderedTitles;

	public LeadershipStructureNode(LeadershipStructureNode copyNode)
	{
		ageRange = copyNode.ageRange;
		requiredGender = copyNode.requiredGender;
		genderedTitles = copyNode.genderedTitles;
	}

	public LeadershipStructureNode(Vector2Int ageRange, Gender requiredGender)
	{
		this.ageRange = ageRange;
		this.requiredGender = requiredGender;
	}
}

[System.Serializable]
public class LeadershipTier
{
	public List<LeadershipStructureNode> tier;

	public LeadershipTier()
	{
		tier = new List<LeadershipStructureNode>();
	}

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

	public int Count
	{
		get
		{
			return tier.Count;
		}
	}

	public IEnumerator<LeadershipStructureNode> GetEnumerator()
	{
		return tier.GetEnumerator();
	}
}
