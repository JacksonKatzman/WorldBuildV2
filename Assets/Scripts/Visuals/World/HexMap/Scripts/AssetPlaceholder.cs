using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Terrain
{
    public class AssetPlaceholder : MonoBehaviour
    {
        public enum AssetType { Foliage, Doodad }
        public AssetType assetType;
        public bool canBeDisabled;
        [ShowIf("@this.canBeDisabled")]
        public HexDirection directionLocation;

        public void ToggleShow(bool on)
        {
            gameObject.SetActive(on);
        }
    }
}