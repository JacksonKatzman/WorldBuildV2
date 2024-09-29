using UnityEngine;

namespace Game.GUI.Adventures
{
    [ExecuteInEditMode]
    public class ScaleByOtherObject : MonoBehaviour
    {
        [SerializeField]
        private RectTransform mine;
        [SerializeField]
        private RectTransform other;
        public float maxHeight = 100;
        private void Update()
        {
            //mine.anchorMin = other.anchorMin;
            //mine.anchorMax = other.anchorMax;
            //mine.anchoredPosition = other.anchoredPosition;
            mine.sizeDelta = new Vector2(other.sizeDelta.x, Mathf.Clamp(other.sizeDelta.y, 0, maxHeight));
        }
    }
}
