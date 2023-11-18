using Game.GUI.Wiki;
using Sirenix.OdinInspector;
using TMPro;

namespace Game.Incidents
{
	public class UserInterfaceService : SerializedMonoBehaviour
	{
		public static UserInterfaceService Instance { get; private set; }
		public IncidentWiki incidentWiki;
		public TMP_Text hexCollectionNameText;

		public void Awake()
		{
			if (Instance != null && Instance != this)
			{
				Destroy(this);
			}
			else
			{
				Instance = this;
			}
		}

		public void ToggleHexCollectionName(bool on, string text)
		{
			hexCollectionNameText.enabled = on;
			hexCollectionNameText.text = text;
		}
	}
}