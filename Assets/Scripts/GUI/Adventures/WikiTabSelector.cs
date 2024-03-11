using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Adventures
{
    public class WikiTabSelector : SerializedMonoBehaviour
    {
        [HideInInspector]
        public IWikiComponent component;
        public Button button;
        [SerializeField]
        private TMP_Text buttonText;
        private Action onClickAction;

        public void Setup(IWikiComponent component, string buttonText, Action onClickAction)
        {
            this.component = component;
            this.buttonText.text = buttonText;
            this.onClickAction = onClickAction;
        }

        public void OnClick()
        {
            onClickAction?.Invoke();
        }
    }
}
