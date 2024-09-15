using Game.Incidents;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class WikiTableOfContents : WikiComponent<List<string>>
    {
        [SerializeField]
        private TMP_Text tableOfContentsText;
        public Type currentType;
        protected override void Fill(List<string> list)
        {
            tableOfContentsText.text = "";
            foreach (var item in list)
            {
                tableOfContentsText.text += $"-{item}\n";
            }
        }

        override public void Clear()
        {
            tableOfContentsText.text = string.Empty;
        }
    }
}
