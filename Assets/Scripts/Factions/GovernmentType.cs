using Game.Factions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = nameof(GovernmentType), menuName = "ScriptableObjects/Government/" + nameof(GovernmentType), order = 1)]
public class GovernmentType : ScriptableObject
{
	public int leadershipSlots;
	public List<GovernmentTrait> traits;
	public int influenceRequirement = 10;
}
