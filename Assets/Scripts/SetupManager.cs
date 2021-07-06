using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Enums;

public class SetupManager : MonoBehaviour
{
    [SerializeField]
    List<NameFormat> nameFormats;

    List<NameContainer> nameContainers;

    Texture2D noiseTexture;
    void Start()
    {
        nameContainers = new List<NameContainer>();

        foreach(NameFormat format in nameFormats)
		{
            nameContainers.Add(new NameContainer(format));
		}

        for(int a = 0; a < 50; a++)
		{
            string fullName = NameGenerator.GeneratePersonFullName(nameContainers[0], Gender.NEITHER);
            Debug.Log(fullName);
		}
    }

    
}
