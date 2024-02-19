using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Game.GUI.Popups
{
	abstract public class Popup : SerializedMonoBehaviour, IPopup
	{
		public abstract Type PopupConfigType { get; }

		[SerializeField]
		private Button closeButton;
		public void ClosePopup()
		{
			PopupService.Instance.ClosePopup(this);
		}

		abstract public void Setup(IPopupConfig config);
		abstract public bool CompareTo(IPopupConfig config);

		protected void ToggleListOfGameObjects(List<GameObject> objects, bool toggle)
		{
			foreach(var obj in objects)
			{
				obj.SetActive(toggle);
			}
		}
	}
}
