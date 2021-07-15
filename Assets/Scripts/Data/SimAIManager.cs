using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Factions;
using Game.Enums;
using Game.People;

public class SimAIManager : MonoBehaviour
{
    private static SimAIManager instance;
    public static SimAIManager Instance => instance;

    [SerializeField]
    private TextAsset factionActionScores;

    private List<MethodInfo> personActionList;
    private Dictionary<PriorityType, List<MethodInfo>> factionActionDictionary;

    public void CallPersonAction(Person person)
	{
        var randomIndex = SimRandom.RandomRange(0, personActionList.Count);
        personActionList[randomIndex].Invoke(null, new object[] { person });
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
        CompilePersonActionList();
        CompileFactionActionDictionary();
    }

    private void CompilePersonActionList()
	{
        personActionList = new List<MethodInfo>(typeof(PersonActions).GetMethods().Where(m => !typeof(object)
                                     .GetMethods()
                                     .Select(me => me.Name)
                                     .Contains(m.Name)));

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

}
