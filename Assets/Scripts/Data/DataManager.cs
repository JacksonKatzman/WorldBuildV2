using Game.Generators.Items;
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
    public TextAsset materialInfo;

    List<NameContainer> nameContainers;
    public NameContainer PrimaryNameContainer => nameContainers[0];

    public MaterialGenerator MaterialGenerator;

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

        foreach (NameFormat format in nameFormats)
        {
            nameContainers.Add(new NameContainer(format));
        }

        MaterialGenerator = new MaterialGenerator();
    }

}
