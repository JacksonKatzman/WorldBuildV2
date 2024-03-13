using Game.GUI.Adventures;
using Game.Simulation;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.GUI.Wiki
{
	public class AdventurePathUIComponent : MonoBehaviour
	{
		[SerializeField]
		private TMP_Text pathTitle;

		[ReadOnly]
		public int linkID = -1;

		public void Setup(int id, string pathTitle)
		{
			linkID = id;
			this.pathTitle.text = pathTitle;
		}

		public void SnapToPath()
		{
			AdventureGuide.Instance.SetCurrentComponent(linkID);
		}

		public void ReplaceTextPlaceholders(List<IAdventureContextRetriever> contexts)
		{
			foreach(var context in contexts)
			{
				var currentText = pathTitle.text;
				context.ReplaceTextPlaceholders(ref currentText);
				pathTitle.text = currentText;
			}
		}
	}
}
