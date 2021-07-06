using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/NameFormat", order = 1)]
public class NameFormat : ScriptableObject
{
    [SerializeField]
    public NamingRules rules;

    [SerializeField]
    public TextAsset maleNames;
    [SerializeField]
    public TextAsset femaleNames;
    [SerializeField]
    public TextAsset staticMaleNames;
    [SerializeField]
    public TextAsset staticFemaleNames;
    [SerializeField]
    public TextAsset vowels;
    [SerializeField]
    public TextAsset consonants;
    [SerializeField]
    public TextAsset startPairs;
    [SerializeField]
    public TextAsset endPairs;
}
