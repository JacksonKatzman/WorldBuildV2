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
    public MeshRenderer meshRenderer;
    public Material opaqueMat;

    private bool highlighted;
    private Material originalMat;
    void Start()
    {
        originalMat = meshRenderer.sharedMaterial;
    }

    public bool OnHighlightStart()
	{
        if (!highlighted)
        {
            highlighted = true;
            if (collection != null && meshRenderer.enabled == true)
            {
                meshRenderer.sharedMaterial = opaqueMat;
                UserInterfaceService.Instance.ToggleHexCollectionName(true, collection.Name);
            }
            return true;
        }
        return false;
	}

    public bool OnHighlightEnd()
	{
        if (highlighted)
        {
            highlighted = false;
            meshRenderer.sharedMaterial = originalMat;
            if (collection != null)
            {
                //UserInterfaceService.Instance.ToggleHexCollectionName(false, collection.Name);
            }
            return true;
        }
        return false;
    }
}
