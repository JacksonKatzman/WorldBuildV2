using Game.GUI.Wiki;
using Sirenix.OdinInspector;

namespace Game.Incidents
{
	public class UserInterfaceService : SerializedMonoBehaviour
	{
		public static UserInterfaceService Instance { get; private set; }
		public IncidentWiki incidentWiki;

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
	}
}