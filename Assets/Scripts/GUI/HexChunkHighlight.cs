using Game.Incidents;
using Game.Terrain;
using HighlightPlus;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexChunkHighlight : MonoBehaviour
{
    [ReadOnly]
    public HexCollection collection;
    public HighlightEffect highlightEffect;
    void Start()
    {
        //highlightEffect.OnObjectHighlightStart += OnHighlightStart;
        //highlightEffect.OnObjectHighlightEnd += OnHighlightEnd;
    }

    public bool OnHighlightStart()
	{
        if (collection != null)
        {
            UserInterfaceService.Instance.ToggleHexCollectionName(true, collection.Name);
        }
        return true;
	}

    private bool OnHighlightEnd(GameObject obj)
	{
        if (collection != null)
        {
            //UserInterfaceService.Instance.ToggleHexCollectionName(false, collection.Name);
        }
        return true;
    }
}
