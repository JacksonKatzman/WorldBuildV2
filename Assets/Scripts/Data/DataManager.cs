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
    private List<NameFormat> nameFormats;

    [SerializeField]
    private List<GovernmentType> governmentTypes;

    [SerializeField]
    private List<Race> races;

    [SerializeField]
    public TextAsset materialInfo;

    List<NameContainer> nameContainers;
    public NameContainer PrimaryNameContainer => nameContainers[0];

    public MaterialGenerator MaterialGenerator;

    private Dictionary<int, List<Race>> weightedRaceDictionary;

    public GovernmentType GetGovernmentType(int influence)
	{
        List<GovernmentType> possibleTypes = new List<GovernmentType>();
        foreach(GovernmentType type in governmentTypes)
		{
            if(type.influenceRequirement <= influence)
			{
                possibleTypes.Add(type);
			}
		}

        var randomIndex = SimRandom.RandomRange(0, possibleTypes.Count);
        return possibleTypes[randomIndex];
	}

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
        nameContainers = new List<NameContainer>();

        BuildWeightedRaceDictionary();

        foreach (NameFormat format in nameFormats)
        {
            nameContainers.Add(new NameContainer(format));
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
}
