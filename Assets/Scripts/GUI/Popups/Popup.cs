using System;
using System.Collections.Generic;
using UnityEngine;

namespace Game.GUI.Popups
{
	abstract public class Popup : MonoBehaviour, IPopup
	{
		public abstract Type PopupConfigType { get; }
		public void ClosePopup()
		{
			PopupService.Instance.ClosePopup(this);
		}

		abstract public void Setup(IPopupConfig config);
		abstract public bool CompareTo(IPopupConfig config);
	}
}
