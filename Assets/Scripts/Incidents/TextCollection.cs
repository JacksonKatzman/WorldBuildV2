using Game.Utilities;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Incidents
{
	[CreateAssetMenu(fileName = nameof(TextCollection), menuName = "ScriptableObjects/Data/" + nameof(TextCollection), order = 1)]
	public class TextCollection : SerializedScriptableObject
	{
		public List<TextAsset> texts;

		public string resourcePath;

		[Button("Compile Objects")]
		private void CompileObjects()
		{
			var files = Directory.GetFiles(Application.dataPath + $"/Resources/{resourcePath}/", "*.json");
			texts = new List<TextAsset>();
			foreach (string file in files)
			{
				var title = file.Replace(Application.dataPath + "/Resources/", "").Replace(".json", "");
				var asset = Resources.Load<TextAsset>(title);
				texts.Add(asset);
			}
		}
	}
}