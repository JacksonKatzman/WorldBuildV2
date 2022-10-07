using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Factions;
using Game.Enums;
using Game.WorldGeneration;
using Game.Data.EventHandling;

public class SimAIManager : MonoBehaviour
{
    private static SimAIManager instance;
    public static SimAIManager Instance => instance;

    [SerializeField]
    private TextAsset factionActionScores;

    private List<MethodInfo> standardPersonEvents;

    private List<MethodInfo> loreEvents;

    private Dictionary<RoleType, List<MethodInfo>> roleBasedPersonEvents;
    private Dictionary<PriorityType, List<MethodInfo>> factionActionDictionary;

    private Dictionary<Type, Dictionary<int, List<MethodInfo>>> weightedLeaderActions;

    private Dictionary<int, List<MethodInfo>> weightedGovernmentUpgrades;
    private Dictionary<int, List<MethodInfo>> weightedWorldEvents;

    public void CallGovernmentUpgrade(Government government)
	{
        var upgrade = GetRandomSelectionFromWeightedDictionary(weightedGovernmentUpgrades);
        upgrade.Invoke(null, new object[] { government });
    }

    public MethodInfo GetRandomLoreEvent()
	{
        var randomIndex = SimRandom.RandomRange(0, loreEvents.Count);
        return loreEvents[randomIndex];
    }

    private bool CallFactionActionByPriorityType(PriorityType priorityType, Faction faction)
	{
        if (priorityType != PriorityType.MILITARY && factionActionDictionary[priorityType].Count > 0)
        {
            var possibleActions = factionActionDictionary[priorityType];
            var randomIndex = SimRandom.RandomRange(0, possibleActions.Count);
            possibleActions[randomIndex].Invoke(null, new object[] { faction });
            return true;
        }
        else
		{
            return false;
		}
    }
    
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private void Start()
    {
    }

    private MethodInfo GetRandomSelectionFromWeightedDictionary(Dictionary<int, List<MethodInfo>> weightedDictionary)
	{
        int totalWeight = 0;
        foreach(var weight in weightedDictionary.Keys)
		{
            totalWeight += weight;
		}

        var randomWeight = SimRandom.RandomRange(1, totalWeight+1);
        MethodInfo info = null;

        foreach(var entry in weightedDictionary)
		{
            if((randomWeight -= entry.Key) <= 0)
			{
                info = SimRandom.RandomEntryFromList(entry.Value);
                break;
			}
		}

        return info;
	}

    private Dictionary<int, List<MethodInfo>> CompileWeightedDictionaryByType(Type type)
	{
        var dict = new Dictionary<int, List<MethodInfo>>();
        var methods = CompileEventListByType(type);

        foreach(var method in methods)
		{
            int weight;
            if(Int32.TryParse(method.Name.Substring(method.Name.Length-2, 2), out weight))
			{
                if(!dict.ContainsKey(weight))
				{
                    dict.Add(weight, new List<MethodInfo>());
				}

                dict[weight].Add(method);
			}
            else
			{
                Debug.LogErrorFormat("Event Method: {0} could not be compiled into weighted dictionary. It has no parseable weight suffix.", method.Name);
			}
		}

        return dict;
    }

    private List<MethodInfo> CompileEventListByType(Type type)
	{
        return new List<MethodInfo>(type.GetMethods().Where(m => !typeof(object)
                                     .GetMethods()
                                     .Select(me => me.Name)
                                     .Contains(m.Name)));
    }
}
