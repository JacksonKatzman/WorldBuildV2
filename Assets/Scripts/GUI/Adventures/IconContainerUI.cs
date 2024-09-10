using Game.Debug;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GUI.Adventures
{
    abstract public class IconContainerUI<T> : SerializedMonoBehaviour
    {
        [SerializeField]
        private Dictionary<T, GameObject> icons = new Dictionary<T, GameObject>();
        [SerializeField]
        private GameObject container;

        public void UpdateIcons<V>(List<V> types) where V : T
        {
            foreach (var pair in icons)
            {
                pair.Value.SetActive(false);
            }
            if (types.Count > 0)
            {
                container.SetActive(true);
                foreach (var t in types)
                {
                    if (icons.ContainsKey(t))
                    {
                        icons[t].SetActive(true);
                    }
                    else
                    {
                        OutputLogger.LogError($"Icon with key {t} doesn't exist in {container.name}!");
                    }
                }
            }
            else
            {
                container.SetActive(false);
            }
        }
    }
}
