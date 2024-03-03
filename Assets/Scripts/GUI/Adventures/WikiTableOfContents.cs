using Game.Incidents;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class WikiTableOfContents : WikiComponent<List<IIncidentContext>>
    {
        [SerializeField]
        private TMP_Text tableOfContentsText;
        protected override void Fill(List<IIncidentContext> list)
        {
            tableOfContentsText.text = "";
            foreach (var item in list)
            {
                var linkString = string.Format("<link=\"{0}\">{1}</link>", item.ID, item.Name);
                tableOfContentsText.text += $"-{linkString}\n";
            }
        }
    }
}
