using UnityEngine;

namespace Game.Incidents
{
	public class SerializedObjectCollectionService
	{
		public SerializedObjectCollectionContainer container => AssetService.Instance.objectData;

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

		}
	}
}