using Sirenix.OdinInspector;
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
	}
}
