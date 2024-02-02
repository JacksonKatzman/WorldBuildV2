using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Terrain
{
    public class AssetPlaceholder : MonoBehaviour
    {
        public enum AssetType { Foliage, Doodad }
        public AssetType assetType;
        public Vector3 baseScale = new Vector3(1.0f, 1.0f, 1.0f);

        public void ToggleShow(bool on)
        {
            gameObject.SetActive(on);
        }
    }
}