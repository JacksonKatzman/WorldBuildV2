using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Terrain
{
    public class ConfigurableHexTerrain : MonoBehaviour
    {
        public List<AssetPlaceholder> placeholders;
        public List<GameObject> doodads;

        public List<GameObject> roadOnlyBaseTemplates;
        public List<GameObject> roadOnlyRoadTemplates;
        public List<GameObject> roadOnlyOffTemplates;
        public List<GameObject> fullOffTemplates;
        public List<GameObject> riverOnlyStraightTemplates;
        public List<GameObject> riverOnlyCurvedTemplates;
        public List<GameObject> riverOnlySharpTemplates;
        public List<GameObject> riverOnlyStartTemplates;
        public List<GameObject> riverOnlyOffTemplates;
        public List<GameObject> riverAndRoadsStraightTemplates;
        public List<GameObject> riverAndRoadsCurvedTemplates;
        public List<GameObject> riverAndRoadsSharpTemplates;
        public List<GameObject> riverAndRoadsRoadTemplates;

        /*
        public GameObject fullOffPos1, fullOffPos2, fullOffPos3, fullOffPos4, fullOffPos5, fullOffPos6;
        //roads only
        public GameObject roadOnlyBase;
        public GameObject roadOnlyRoadPos1, roadOnlyRoadPos2, roadOnlyRoadPos3, roadOnlyRoadPos4, roadOnlyRoadPos5, roadOnlyRoadPos6;
        public GameObject roadOnlyOffPos1, roadOnlyOffPos2, roadOnlyOffPos3, roadOnlyOffPos4, roadOnlyOffPos5, roadOnlyOffPos6;
        //rivers only
        public GameObject riverOnlyStraight, riverOnlyCurved, riverOnlySharp;
        public GameObject riverOnlyOffPos1, riverOnlyOffPos2, riverOnlyOffPos3, riverOnlyOffPos4, riverOnlyOffPos5, riverOnlyOffPos6;
        */
        //roads and rivers

        private void Awake()
        {
            placeholders = GetComponentsInChildren<AssetPlaceholder>().ToList();
            placeholders.ForEach(x => x.ToggleShow(false));
        }
    }
}