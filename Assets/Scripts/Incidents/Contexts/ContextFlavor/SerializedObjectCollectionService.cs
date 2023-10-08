using UnityEngine;

namespace Game.Incidents
{
	public class SerializedObjectCollectionService
	{
		public SerializedObjectCollectionContainer container;

		private static SerializedObjectCollectionService instance;
		public static SerializedObjectCollectionService Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new SerializedObjectCollectionService();
				}
				return instance;
			}
		}

		private SerializedObjectCollectionService()
		{
			container = Resources.Load<SerializedObjectCollectionContainer>("ScriptableObjects/Data/ObjectData");
		}
	}
}