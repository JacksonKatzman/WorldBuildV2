using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Terrain
{
    public class ConfigurableHexTerrain : MonoBehaviour
    {
        public List<AssetPlaceholder> placeholders;
        public List<GameObject> doodads;
        private void Awake()
        {
            placeholders = GetComponentsInChildren<AssetPlaceholder>().ToList();
            placeholders.ForEach(x => x.ToggleShow(false));
        }
    }
}