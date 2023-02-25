using Game.GUI.Wiki;
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
			var keywordObjects = Resources.LoadAll<AdventureKeyword>("ScriptableObjects/Keywords");
			foreach(var o in keywordObjects)
			{
				keywords.Add(o.keyword, o);
			}
		}
	}
}
