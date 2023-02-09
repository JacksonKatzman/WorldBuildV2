using Game.Incidents;
using Game.Simulation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.GUI.Wiki
{
	public class IncidentWiki : Wiki
	{
		private bool initialized;

		public void InitializeWiki()
		{
			if(!initialized)
			{
				currentTab = MakeTab();
				tabs.Add(currentTab);
				OpenPage(0);

				initialized = true;
			}
		}

		protected override void OnLinkClick(string linkID)
		{
			var id = Int32.Parse(linkID);
			if (id != currentTab.currentPage.Value.contextID)
			{
				if (Input.GetKey(KeyCode.LeftControl))
				{
					currentTab = MakeTab();
				}

				OpenPage(id);
			}
		}

		override public void OpenPage(int id)
		{
			if(!pages.ContainsKey(id))
			{
				//Get the context in question
				var context = id == 0 ? SimulationManager.Instance.world : SimulationManager.Instance.AllContexts.GetContextByID(id);
				if (context != null)
				{
					//make a new page
					var page = Instantiate(wikiPagePrefab, pageRoot).GetComponent<WikiPage>();
					page.contextID = id;
					page.wikiTitle.text = context.Name;

					if (id == 0)
					{
						foreach (var item in IncidentService.Instance.reports)
						{
							AddReportToPage(page, item);
						}
					}
					else
					{
						foreach (var item in IncidentService.Instance.reports)
						{
							if (item.Contexts.Values.Contains(context))
							{
								AddReportToPage(page, item);
							}
						}
					}

					pages.Add(id, page);
				}
			}

			currentTab.SwitchToPage(pages[id]);
		}

		private void AddReportToPage(WikiPage page, IncidentReport report)
		{
			page.wikiText.text += report.ReportYear + ": ";
			page.wikiText.text += report.GenerateLinkedLog();
			page.wikiText.text += "\n";
		}
	}
}
