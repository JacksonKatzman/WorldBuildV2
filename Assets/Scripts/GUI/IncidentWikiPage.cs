using TMPro;
using UnityEngine;

namespace Game.GUI.Wiki
{
	public class IncidentWikiPage : MonoBehaviour
	{
		[SerializeField]
		public TMP_Text wikiTitle;
		[SerializeField]
		public TMP_Text wikiText;
		[SerializeField]
		public RectTransform contentRect;

		public int contextID;
	}
}
