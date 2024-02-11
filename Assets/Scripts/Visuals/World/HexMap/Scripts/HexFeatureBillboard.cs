using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Terrain
{
    public class HexFeatureBillboard : MonoBehaviour
    {
        Camera mainCamera;
        public TMP_Text tmpText;

        private void Start()
        {
            mainCamera = Camera.main;
        }

        private void LateUpdate()
        {
            //transform.LookAt(mainCamera.transform);
            //transform.Rotate(0, 180, 0);
            transform.rotation = mainCamera.transform.rotation;
        }
    }
}