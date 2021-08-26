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

    public static Dictionary<PriorityType, Type> PriorityTypeMap = new Dictionary<PriorityType, Type>
    {
        { PriorityType.MILITARY, typeof(LeaderActions_Military)},
        { PriorityType.INFRASTRUCTURE, typeof(LeaderActions_Infrastructure)},
        { PriorityType.MERCANTILE, typeof(LeaderActions_Mercantile)},
        { PriorityType.POLITICAL, typeof(LeaderActions_Political)},
        { PriorityType.RELIGIOUS, typeof(LeaderActions_Religious)}
    };

    [SerializeField]
    private TextAsset factionActionScores;

    private List<MethodInfo> standardPersonEvents;

    private List<MethodInfo> loreEvents;

    private Dictionary<RoleType, List<MethodInfo>> roleBasedPersonEvents;
    private Dictionary<PriorityType, List<MethodInfo>> factionActionDictionary;

    private Dictionary<Type, Dictionary<int, List<MethodInfo>>> weightedLeaderActions;

    private Dictionary<int, List<MethodInfo>> weightedGovernmentUpgrades;
    private Dictionary<int, List<MethodInfo>> weightedWorldEvents;

    public void CallPersonEvent(Person person)
	{
        var index = person.roles.Contains(RoleType.GOVERNER) ? 0.67f : 0.0f;
        var randomIndex = SimRandom.RandomFloat01();

        if(randomIndex >= index)
		{
            var possibleEvents = new List<MethodInfo>();
            possibleEvents.AddRange(standardPersonEvents);

            foreach (var role in person.roles)
            {
                possibleEvents.AddRange(roleBasedPersonEvents[role]);
            }

            SimRandom.RandomEntryFromList(possibleEvents).Invoke(null, new object[] { person });
        }
        else
		{
            var totalPriorities = 0;
            foreach(var pair in person.priorities.priorities)
			{
                totalPriorities += pair.Value;
			}

            var randomPriority = SimRandom.RandomRange(0, totalPriorities);

            foreach (var pair in person.priorities.priorities)
            {
                if((randomPriority -= pair.Value) <= 0)
				{
                    var chosenSubset = weightedLeaderActions[PriorityTypeMap[pair.Key]];
                    var chosenAction = GetRandomSelectionFromWeightedDictionary(chosenSubset);
                    chosenAction.Invoke(null, new object[] { person });
                }
            }
        }
    }

    public void CallFactionActionByScores(Priorities score, Faction faction)
	{
        int index = 0;
        int tries = 0;
        int actionsTaken = 0;
        var sortedList = score.SortedList();

        while(actionsTaken < faction.actionsRemaining && tries < 10)
		{
            if(CallFactionActionByPriorityType(sortedList[index], faction))
			{
                actionsTaken++;
			}

            index++;
            tries++;

            if(index >= sortedList.Count)
			{
                index = 0;
			}
		}            
	}

    public void CallWorldEvent(World world)
	{
        var worldEvent = GetRandomSelectionFromWeightedDictionary(weightedWorldEvents);
        worldEvent.Invoke(null, new object[] { world });
    }

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
        CompilePersonEvents();
        CompileFactionActionDictionary();
        CompileWorldEvents();

        weightedGovernmentUpgrades = CompileWeightedDictionaryByType(typeof(GovernmentEvents));
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

    private void CompilePersonEvents()
	{
        standardPersonEvents = CompileEventListByType(typeof(PersonEvents));

        roleBasedPersonEvents = new Dictionary<RoleType, List<MethodInfo>>();

        roleBasedPersonEvents.Add(RoleType.GOVERNER, CompileEventListByType(typeof(LeaderActions_Misc)));
        roleBasedPersonEvents.Add(RoleType.MAGIC_USER, new List<MethodInfo>());
        roleBasedPersonEvents.Add(RoleType.ROGUE, new List<MethodInfo>());

        weightedLeaderActions = new Dictionary<Type, Dictionary<int, List<MethodInfo>>>();

        weightedLeaderActions.Add(typeof(LeaderActions_Military), CompileWeightedDictionaryByType(typeof(LeaderActions_Military)));
        weightedLeaderActions.Add(typeof(LeaderActions_Infrastructure), CompileWeightedDictionaryByType(typeof(LeaderActions_Infrastructure)));
        weightedLeaderActions.Add(typeof(LeaderActions_Mercantile), CompileWeightedDictionaryByType(typeof(LeaderActions_Mercantile)));
        weightedLeaderActions.Add(typeof(LeaderActions_Political), CompileWeightedDictionaryByType(typeof(LeaderActions_Political)));
        weightedLeaderActions.Add(typeof(LeaderActions_Religious), CompileWeightedDictionaryByType(typeof(LeaderActions_Religious)));
    }

    private void CompileFactionActionDictionary()
	{
        factionActionDictionary = new Dictionary<PriorityType, List<MethodInfo>>();

        factionActionDictionary.Add(PriorityType.MILITARY, null);
        factionActionDictionary.Add(PriorityType.INFRASTRUCTURE, new List<MethodInfo>(typeof(InfrastructureActions).GetMethods().Where(m => !typeof(object)
                                     .GetMethods()
                                     .Select(me => me.Name)
                                     .Contains(m.Name))));
        factionActionDictionary.Add(PriorityType.MERCANTILE, new List<MethodInfo>(typeof(MercantileActions).GetMethods().Where(m => !typeof(object)
                                     .GetMethods()
                                     .Select(me => me.Name)
                                     .Contains(m.Name))));
        factionActionDictionary.Add(PriorityType.POLITICAL, new List<MethodInfo>(typeof(PoliticalActions).GetMethods().Where(m => !typeof(object)
                                     .GetMethods()
                                     .Select(me => me.Name)
                                     .Contains(m.Name))));
        factionActionDictionary.Add(PriorityType.RELIGIOUS, new List<MethodInfo>(typeof(ReligiousActions).GetMethods().Where(m => !typeof(object)
                                     .GetMethods()
                                     .Select(me => me.Name)
                                     .Contains(m.Name))));
    }

    private void CompileWorldEvents()
	{
        loreEvents = new List<MethodInfo>(typeof(LoreEvents).GetMethods().Where(m => !typeof(object)
                                     .GetMethods()
                                     .Select(me => me.Name)
                                     .Contains(m.Name)));
        weightedWorldEvents = CompileWeightedDictionaryByType(typeof(WorldEvents));
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
