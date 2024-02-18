using Sirenix.OdinInspector;
using UnityEngine;

namespace Game.Terrain
{
    public class ColliderRemover : MonoBehaviour
    {
        [Button("Disable Colliders")]
        public void DisableColliders()
        {
            var meshColliders = GetComponentsInChildren<MeshCollider>();
            var boxColliders = GetComponentsInChildren<BoxCollider>();

            foreach(var col in meshColliders)
            {
                col.enabled = false;
            }

            foreach(var col in boxColliders)
            {
                col.enabled = false;
            }
        }
    }
}