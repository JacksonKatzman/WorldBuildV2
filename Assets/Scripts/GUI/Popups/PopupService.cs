using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.GUI.Popups
{
	public class PopupService : SerializedMonoBehaviour
	{
		public static PopupService Instance { get; private set; }
		private void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
				Setup();
			}
		}

		[SerializeField]
		private List<PopupDefinition> popupDefinitions;

		[SerializeField]
		private Transform activePopupAnchor;

		[SerializeField]
		private Transform recycledPopupAnchor;

		private List<Popup> activePopups;
		private List<Popup> recycledPopups;

		private void Setup()
		{
			activePopups = new List<Popup>();
			recycledPopups = new List<Popup>();
		}

		public void ShowPopup(IPopupConfig config)
		{
			foreach (var activePopup in activePopups)
			{
				if (activePopup.CompareTo(config))
				{
					activePopup.GetComponent<RectTransform>().SetAsLastSibling();
					return;
				}
			}

			var popup = GetOrCreatePopup(config);
			popup.transform.parent = activePopupAnchor;
			popup.Setup(config);
			popup.gameObject.SetActive(true);
			activePopups.Add(popup);
		}

		public void ClosePopup(Popup popup)
		{
			popup.transform.parent = recycledPopupAnchor;
			popup.gameObject.SetActive(false);
			activePopups.Remove(popup);
			recycledPopups.Add(popup);
		}

		private Popup GetOrCreatePopup(IPopupConfig config)
		{
			var reusablePopups = recycledPopups.Where(x => x.PopupConfigType == config.GetType()).ToList();
			if(reusablePopups.Count() > 0)
			{
				var popup = reusablePopups.First();
				recycledPopups.Remove(popup);
				return popup;
			}
			else
			{
				var prefab = popupDefinitions.First(x => x.PopupType == config.PopupType).Prefab;
				var popupObject = Instantiate(prefab, recycledPopupAnchor);
				return popupObject.GetComponent<Popup>();
			}
		}
	}
}