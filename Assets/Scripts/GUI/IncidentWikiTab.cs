using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GUI.Wiki
{
	public class IncidentWikiTab : MonoBehaviour
	{
		public LinkedList<IncidentWikiPage> pageHistory;
		public LinkedListNode<IncidentWikiPage> currentPage;
		public Action<IncidentWikiTab> onButtonClicked;

		public IncidentWikiTab()
		{
			pageHistory = new LinkedList<IncidentWikiPage>();
		}

		public void SwitchToTab()
		{
			onButtonClicked?.Invoke(this);
		}

		public void GoToPreviousPage()
		{
			if (currentPage.Previous != null)
			{
				currentPage.Value.gameObject.SetActive(false);
				currentPage = currentPage.Previous;
				currentPage.Value.gameObject.SetActive(true);
			}
		}

		public void GoToNextPage()
		{
			if (currentPage.Next != null)
			{
				currentPage.Value.gameObject.SetActive(false);
				currentPage = currentPage.Next;
				currentPage.Value.gameObject.SetActive(true);
			}
		}

		public void SwitchToPage(IncidentWikiPage incidentWikiPage)
		{
			if (currentPage != null)
			{
				currentPage.Value.gameObject.SetActive(false);
			}

			if (currentPage != null && currentPage.Next != null)
			{
				var last = pageHistory.Last;
				while (last != currentPage)
				{
					last = last.Previous;
					pageHistory.RemoveLast();
				}
			}

			pageHistory.AddLast(incidentWikiPage);
			currentPage = pageHistory.Last;
			currentPage.Value.gameObject.SetActive(true);
		}
	}
}
