using Sirenix.OdinInspector;

namespace Game.Incidents
{
	public class AssetService : SerializedMonoBehaviour
	{
		public static AssetService Instance { get; private set; }
		public TextCollection incidents;
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