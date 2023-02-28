using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

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

		private List<Popup> activePopups;
		private List<Popup> recycledPopups;

		private void Setup()
		{
			activePopups = new List<Popup>();
			recycledPopups = new List<Popup>();
		}
	}
}