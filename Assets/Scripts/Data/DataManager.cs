using Game.Creatures;
using Game.Generators.Items;
using Game.Races;
using System.Collections;
using System.Collections.Generic;
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

    private Dictionary<int, List<Race>> weightedRaceDictionary;

    private Dictionary<string, CreatureStats> creatures;

    public Race GetRandomWeightedRace()
	{
        return SimRandom.RandomEntryFromWeightedDictionary(weightedRaceDictionary);
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
        var existingCreatures = Resources.LoadAll("Creatures/Creatures");
        creatures = new Dictionary<string, CreatureStats>();

        if (existingCreatures != null)
        {
            foreach (var c in existingCreatures)
            {
                creatures.Add(((CreatureStats)c).Name, ((CreatureStats)c));
            }
        }
    }
}
