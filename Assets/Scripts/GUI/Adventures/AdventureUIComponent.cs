using Game.Simulation;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.GUI.Wiki
{
	public abstract class AdventureUIComponent : SerializedMonoBehaviour, IAdventureUIComponent
	{
		protected static byte FULL_ALPHA = 255;
		protected static byte FADED_ALPHA = 100;

		public static float DOUBLE_CLICK_THRESHOLD = 0.2f;
		private float lastClickTime = -1;

		virtual public bool Completed { get; set; }
		public void ToggleCompleted()
		{
			Completed = !Completed;
			ToggleElements();
		}

		abstract protected void ToggleElements();

		public void OnPointerClick()
		{
			OutputLogger.Log("CLICK!");
			if(Time.time - lastClickTime < DOUBLE_CLICK_THRESHOLD)
			{
				OutputLogger.Log("DOUBLE CLICK!");
				ToggleCompleted();
			}

			lastClickTime = Time.time;
		}

		protected Color32 SwapColorAlpha(Color32 color, byte alpha)
		{
			return new Color32(color.r, color.g, color.b, alpha);
		}

		abstract public void BuildUIComponents(IAdventureComponent component);
	}
}
