using Game.Incidents;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class AllIncidentsWiki : WikiComponent<List<IncidentReport>>
    {
        [SerializeField]
        public TMP_Text content;
        protected override void Fill(List<IncidentReport> value)
        {
            content.text = "";
            foreach (var item in value)
            {
                if (item.IsMajorIncident)
                {
                    AddReportToPage(item);
                }
            }
        }

        private void AddReportToPage(IncidentReport report)
        {

            content.text += report.ReportYear + ": ";
            content.text += report.ReportHeadline;
            content.text += "\n";
        }
    }
}
