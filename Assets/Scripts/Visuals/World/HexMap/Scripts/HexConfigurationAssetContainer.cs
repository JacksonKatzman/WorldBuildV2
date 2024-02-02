using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Terrain
{
    [CreateAssetMenu(fileName = nameof(HexConfigurationAssetContainer), menuName = "ScriptableObjects/Biomes/" + nameof(HexConfigurationAssetContainer), order = 1)]
    public class HexConfigurationAssetContainer : SerializedScriptableObject
    {
        public List<GameObject> roadsOnly;
    }
}