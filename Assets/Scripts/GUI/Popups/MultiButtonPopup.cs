using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Popups
{
    public class MultiButtonPopup : Popup
    {
        public override Type PopupConfigType => typeof(MultiButtonPopupConfig);

        [SerializeField]
        private TMP_Text title;
        [SerializeField]
        private TMP_Text description;
        [SerializeField]
        private Transform buttonAnchor;
        [SerializeField]
        private Button buttonPrefab;

        private List<Button> currentButtons;

        public override bool CompareTo(IPopupConfig config)
        {
            return config.GetType() == PopupConfigType;
        }

        public override void Setup(IPopupConfig config)
        {
            var typedConfig = config as MultiButtonPopupConfig;
            var actions = typedConfig.ButtonActions;
            Clear();

            title.text = typedConfig.Title;
            description.text = typedConfig.Description;

            foreach(var pair in actions)
            {
                var button = Instantiate(buttonPrefab, buttonAnchor);
                var actionName = pair.Key;
                var action = pair.Value;
                button.GetComponentInChildren<TMP_Text>().text = actionName;
                button.onClick.AddListener(() => 
                {
                    if(typedConfig.CloseOnButtonPress)
                    {
                        ClosePopup();
                    }
                    action.Invoke(); 
                });

                currentButtons.Add(button);
            }
        }

        private void Clear()
        {
            if (currentButtons == null)
            {
                currentButtons = new List<Button>();
            }
            else
            {
                for (int i = 0; i < currentButtons.Count; i++)
                {
                    Destroy(currentButtons[i].gameObject);
                }
                currentButtons.Clear();
            }
        }
    }
}
