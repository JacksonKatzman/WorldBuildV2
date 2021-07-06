using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NamingRules", order = 2)]
public class NamingRules : ScriptableObject
{
    public bool TweakFirstNames;
    public Vector2Int FirstNameSyllables;
    public Vector2Int LastNameSyllables;
}