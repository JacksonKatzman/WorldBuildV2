using Cysharp.Threading.Tasks;
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
		[HideInInspector]
		public bool initialized;

		public async UniTask InitializeWiki()
		{
			if(!initialized)
			{
				OpenPage(0);

				initialized = true;
				await UniTask.Yield();
			}
		}

		public void Clear()
		{
			foreach(var pair in pages)
			{
				if (pair.Value.contextID != 0)
				{
					Destroy(pair.Value);
				}
			}

			pages.Clear();

			foreach(var tab in tabs)
			{
				Destroy(tab);
			}

			tabs.Clear();
			currentTab = MakeTab();
			tabs.Add(currentTab);
			initialized = false;
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

		public bool BuildPage(int id)
		{
			if (!pages.ContainsKey(id))
			{
				//make a new page
				var page = Instantiate(wikiPagePrefab, pageRoot).GetComponent<WikiPage>();
				page.contextID = id;

				pages.Add(id, page);
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool PopulatePage(int id)
		{
			var context = id == 0 ? SimulationManager.Instance.world : SimulationManager.Instance.AllContexts.GetContextByID(id);
			if (context != null && pages.ContainsKey(id))
			{
				var page = pages[id];
				page.wikiTitle.text = context.Name;

				if (id == 0)
				{
					foreach (var item in IncidentService.Instance.reports)
					{
						if (item.IsMajorIncident)
						{
							AddReportToPage(page, item);
						}
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
				return true;
			}
			else
			{
				return false;
			}
		}

		override public void OpenPage(int id)
		{
			if(!pages.ContainsKey(id))
			{
				BuildPage(id);
				PopulatePage(id);
			}

			currentTab.SwitchToPage(pages[id]);
		}

		public void ToggleView(bool on)
		{
			currentTab.ToggleView(on);
		}

		private void AddReportToPage(WikiPage page, IncidentReport report)
		{
			
			page.wikiText.text += report.ReportYear + ": ";
			page.wikiText.text += report.ReportLog;
			page.wikiText.text += "\n";
		}
	}
}
