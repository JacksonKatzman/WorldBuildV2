using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Terrain
{
    public class PlaceholderScaleSetter : MonoBehaviour
    {
        public float scale = 1.0f;
        [Button("Set Scale")]
        public void MassSetScale()
        {
            foreach (var placeholder in GetComponentsInChildren<AssetPlaceholder>(true))
            {
                placeholder.baseScale = new Vector3(scale, scale, scale);
            }
        }
    }
}