using UnityEngine;

namespace Game.Incidents
{
	public class SerializedObjectCollectionService
	{
		public SerializedObjectCollectionContainer Container { get; private set; }

		private static SerializedObjectCollectionService instance;
		public static SerializedObjectCollectionService Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new SerializedObjectCollectionService();
					GetContainer();
				}
				return instance;
			}
		}

		private SerializedObjectCollectionService()
		{

		}

		private static void GetContainer()
		{
			if(AssetService.Instance != null)
			{
				Instance.Container = AssetService.Instance.objectData;
			}
			else
			{
				Instance.Container = Resources.Load<SerializedObjectCollectionContainer>("ScriptableObjects/Data/ObjectData");
			}
		}
	}
}