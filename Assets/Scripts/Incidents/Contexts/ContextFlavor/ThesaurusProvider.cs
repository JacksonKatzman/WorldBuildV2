using Game.Utilities;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Game.Incidents
{
	static public class ThesaurusProvider
	{
		private static string THESAURUS_FILE_NAME = "thesaurus.json";
		public static Dictionary<string, List<string>> Thesaurus
		{
			get
			{
				if(thesaurus == null || thesaurus.Count == 0)
				{
					LoadThesaurus();
				}
				return thesaurus;
			}
			private set
			{
				thesaurus = value;
			}
		}
		private static Dictionary<string, List<string>> thesaurus;

		public static void LoadThesaurus()
		{
			var dataPath = Path.Combine(Application.dataPath + SaveUtilities.THESAURUS_DATA_PATH);
			var files = Directory.GetFiles(dataPath, THESAURUS_FILE_NAME);
			if(files.Length <= 0)
			{
				thesaurus = new Dictionary<string, List<string>>();
				return;
			}
			var file = files[0];
			var text = File.ReadAllText(file);

			thesaurus = JsonConvert.DeserializeObject<Dictionary<string, List<string>>>(text, SaveUtilities.SERIALIZER_SETTINGS);
		}

		public static void SaveThesaurus()
		{
			if(thesaurus == null)
			{
				return;
			}
			var path = Path.Combine(Application.dataPath + SaveUtilities.THESAURUS_DATA_PATH + THESAURUS_FILE_NAME);
			string output = JsonConvert.SerializeObject(thesaurus, Formatting.Indented, SaveUtilities.SERIALIZER_SETTINGS);
			File.WriteAllText(path, output);
#if UNITY_EDITOR
			AssetDatabase.Refresh();
#endif
		}

		public static bool AddEntry(string key, List<string> entries)
		{
			key = key.ToUpper();
			if(!Thesaurus.ContainsKey(key))
			{
				Thesaurus.Add(key, entries);
				Thesaurus[key].Add(key.ToLower());
			}
			else
			{
				foreach(var entry in entries)
				{
					if(!Thesaurus[key].Contains(entry))
					{
						Thesaurus[key].Add(entry);
					}
				}
			}

			SaveThesaurus();
			return true;
		}

		public static List<string> ConvertEntryToStringList(string entry)
		{
			var actualSynonyms = entry.Split()[0];
			return actualSynonyms.Split('|').ToList();
		}
	}
}