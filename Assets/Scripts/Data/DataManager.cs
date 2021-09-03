using Game.Creatures;
using Game.Enums;
using Game.Generators.Items;
using Game.ModularEvents;
using Game.Races;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager instance;
    public static DataManager Instance => instance;

    [SerializeField]
    private List<Race> races;

    [SerializeField]
    public TextAsset materialInfo;

    public Dictionary<Race, NameContainer> nameContainers;

    public MaterialGenerator MaterialGenerator;

    public List<ModularEventNode> modularEventNodes;

    private Dictionary<int, List<Race>> weightedRaceDictionary;

    private Dictionary<string, Game.Creatures.MonsterStats> creatures;

    public Race GetRandomWeightedRace()
	{
        return SimRandom.RandomEntryFromWeightedDictionary(weightedRaceDictionary);
	}

    public Game.Creatures.MonsterStats GetRandomCreature(bool legendary, bool landDwelling, params CreatureType[] typesArray)
	{
        var types = typesArray.ToList();
        var matches = creatures.Values.Where(c => types.Contains(c.type) && c.legendary == legendary && c.landDwelling == landDwelling).ToList();
        return SimRandom.RandomEntryFromList(matches);
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
        nameContainers = new Dictionary<Race, NameContainer>();

        BuildWeightedRaceDictionary();

        BuildCreatureDictionary();

        BuildModularEventList();

        foreach (var race in races)
        {
            nameContainers.Add(race, new NameContainer(race.nameFormat));
        }

        MaterialGenerator = new MaterialGenerator();
    }

    private void BuildWeightedRaceDictionary()
	{
        weightedRaceDictionary = new Dictionary<int, List<Race>>();

        foreach(var race in races)
		{
            if(!weightedRaceDictionary.ContainsKey(race.appearanceWeight))
			{
                weightedRaceDictionary.Add(race.appearanceWeight, new List<Race>());
			}

            weightedRaceDictionary[race.appearanceWeight].Add(race);
		}
	}

    private void BuildCreatureDictionary()
	{
        var existingCreatures = Resources.LoadAll("ScriptableObjects/Creatures");
		creatures = new Dictionary<string, Game.Creatures.MonsterStats>();

        if (existingCreatures != null)
        {
            foreach (var c in existingCreatures)
            {
				creatures.Add(((Game.Creatures.MonsterStats)c).Name, ((Game.Creatures.MonsterStats)c));
            }
        }
    }

    private void BuildModularEventList()
	{
        var nodes = Resources.LoadAll("ScriptableObjects/ModularEvents");
        modularEventNodes = new List<ModularEventNode>();

        if(nodes != null)
		{
            foreach(var node in nodes)
			{
                modularEventNodes.Add((ModularEventNode)node);
			}
		}
    }
}
