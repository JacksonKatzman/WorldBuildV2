using Game.GUI.Wiki;
using Game.Incidents;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Simulation
{
	public class InfoService
	{
		private static InfoService instance;
		public static InfoService Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new InfoService();
				}
				return instance;
			}
		}

		public static Dictionary<string, AdventureKeyword> Keywords => Instance.keywords;
		private Dictionary<string, AdventureKeyword> keywords;

		public InfoService()
		{
			keywords = new Dictionary<string, AdventureKeyword>();
			var keywordObjects = SerializedObjectCollectionService.Instance.container.collections[typeof(AdventureKeyword)];
			foreach (var pair in keywordObjects.objects)
			{
				keywords.Add(pair.Key, pair.Value as AdventureKeyword);
			}
		}
	}
}
