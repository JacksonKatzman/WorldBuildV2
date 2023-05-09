using System;
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
	}
}
