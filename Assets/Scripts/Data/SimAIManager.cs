using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Factions;
using Game.Enums;

public class SimAIManager : MonoBehaviour
{
    private static SimAIManager instance;
    public static SimAIManager Instance => instance;

    [SerializeField]
    private TextAsset factionActionScores;


    private Dictionary<PriorityType, List<MethodInfo>> eventDictionary;

    
    public void CallActionByScores(Priorities score, Faction faction)
	{
        int index = 0;
        int tries = 0;
        int actionsTaken = 0;
        var sortedList = score.SortedList();

        while(actionsTaken < faction.actionsRemaining && tries < 10)
		{
            if(CallActionByPriorityType(sortedList[index], faction))
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

    private bool CallActionByPriorityType(PriorityType priorityType, Faction faction)
	{
        if (priorityType != PriorityType.MILITARY && eventDictionary[priorityType].Count > 0)
        {
            var possibleActions = eventDictionary[priorityType];
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
        CompileEventDictionary();
        /*
        MethodInfo method = null;
        foreach(ActionKey key in eventDictionary.Keys)
		{
            if(key.fuctionName == "DoNothing")
			{
                method = eventDictionary[key];
                break;
			}
		}
        method?.Invoke(null, new object[] { null });
        */
	}
    /*
    private void CompileEventDictionaries()
	{
        eventDictionary = new Dictionary<ActionKey, MethodInfo>();
        string[] actionScores = factionActionScores.text.Split('\n');

        foreach(var method in typeof(FactionActions).GetMethods())
		{
            var methodName = method.Name;
            foreach(string score in actionScores)
			{
                string[] splitValues = score.Split(',');
                if (splitValues[0] == methodName)
				{
                    var actionScore = new Priorities(Int32.Parse(splitValues[2]), Int32.Parse(splitValues[3]), Int32.Parse(splitValues[4]), Int32.Parse(splitValues[5]), Int32.Parse(splitValues[6]));
                    var actionKey = new ActionKey(splitValues[0], Int32.Parse(splitValues[1]), actionScore);
                    eventDictionary.Add(actionKey, method);
                    break;
                }
			}
		}
    }
    */

    private void CompileEventDictionary()
	{
        eventDictionary = new Dictionary<PriorityType, List<MethodInfo>>();

        eventDictionary.Add(PriorityType.MILITARY, null);
        eventDictionary.Add(PriorityType.INFRASTRUCTURE, new List<MethodInfo>(typeof(InfrastructureActions).GetMethods().Where(m => !typeof(object)
                                     .GetMethods()
                                     .Select(me => me.Name)
                                     .Contains(m.Name))));
        eventDictionary.Add(PriorityType.MERCANTILE, new List<MethodInfo>(typeof(MercantileActions).GetMethods().Where(m => !typeof(object)
                                     .GetMethods()
                                     .Select(me => me.Name)
                                     .Contains(m.Name))));
        eventDictionary.Add(PriorityType.POLITICAL, new List<MethodInfo>(typeof(PoliticalActions).GetMethods().Where(m => !typeof(object)
                                     .GetMethods()
                                     .Select(me => me.Name)
                                     .Contains(m.Name))));
        eventDictionary.Add(PriorityType.EXPANSION, new List<MethodInfo>(typeof(ExpansionActions).GetMethods().Where(m => !typeof(object)
                                     .GetMethods()
                                     .Select(me => me.Name)
                                     .Contains(m.Name))));
        eventDictionary.Add(PriorityType.RELIGIOUS, new List<MethodInfo>(typeof(ReligiousActions).GetMethods().Where(m => !typeof(object)
                                     .GetMethods()
                                     .Select(me => me.Name)
                                     .Contains(m.Name))));
    }

}
