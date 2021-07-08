using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Factions;

public class SimAIManager : MonoBehaviour
{
    private static SimAIManager instance;
    public static SimAIManager Instance => instance;

    [SerializeField]
    private TextAsset factionActionScores;


    private Dictionary<ActionKey, MethodInfo> eventDictionary;

    public void CallActionByScore(Priorities score, Faction faction)
	{
        int currentBestScore = int.MaxValue;
        ActionKey currentBestMethod = null;
       // OutputLogger.LogFormatAndPause("Faction Score: {0} {1} {2} {3} {4}", Game.Enums.LogSource.IMPORTANT, score.militaryScore, score.infrastructureScore, score.mercantileScore, score.politicalScore, score.expansionScore);
        foreach (ActionKey key in eventDictionary.Keys)
		{
            //OutputLogger.LogFormatAndPause("Key Score: {0} {1} {2} {3} {4}", Game.Enums.LogSource.IMPORTANT, key.score.militaryScore, key.score.infrastructureScore, key.score.mercantileScore, key.score.politicalScore, key.score.expansionScore);
            var testedScore = Priorities.CompareScores(key.score, score);
            //OutputLogger.LogFormatAndPause("{0} Faction had a compared score of {1} for the {2} action.", Game.Enums.LogSource.IMPORTANT, faction.name, testedScore, key.fuctionName);
            if(testedScore < currentBestScore)
			{
                currentBestScore = testedScore;
                currentBestMethod = key;
			}
		}

        if(currentBestMethod != null)
		{
            eventDictionary[currentBestMethod].Invoke(null, new object[] { faction });
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
        CompileEventDictionaries();
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
}
