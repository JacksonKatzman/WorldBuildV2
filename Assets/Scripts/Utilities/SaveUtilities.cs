using UnityEngine;
using System.IO;
using Game.Terrain;
using Newtonsoft.Json;
using System.Collections.Generic;
using Game.Simulation;
using Game.Debug;

namespace Game.Utilities
{
	public static class SaveUtilities
	{
		const int mapFileVersion = 4;

		public static string INCIDENT_DATA_PATH = "/Resources/IncidentData/";
		public static string THESAURUS_DATA_PATH = "/Resources/ThesaurusData/";
		public static string ENCOUNTER_DATA_PATH = "/Resources/ScriptableObjects/Encounters";
		public static string RESOURCES_DATA_PATH = "/Resources/";
		public static string SCRIPTABLE_OBJECTS_PATH = "/Resources/ScriptableObjects/";
		public static JsonSerializerSettings SERIALIZER_SETTINGS = new JsonSerializerSettings()
		{
			// ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
			PreserveReferencesHandling = PreserveReferencesHandling.Objects,
			TypeNameHandling = TypeNameHandling.All,
			MissingMemberHandling = MissingMemberHandling.Ignore,
		};

		public static string ROOT = Path.Combine(Application.persistentDataPath, "GameData");

		public static string GetMapRootPath(string mapName)
		{
			return Path.Combine(ROOT, mapName);
		}

		public static string GetHexMapDataPath(string mapName)
		{
			return Path.Combine(ROOT, mapName, "HexMapData");
		}

		public static string GetHexMapData(string mapName)
		{
			return Path.Combine(GetMapRootPath(mapName), mapName + ".map");
		}

		public static string GetSimCellsPath(string mapName)
		{
			return Path.Combine(ROOT, mapName, "SimCells");
		}

		public static string GetWorldPath(string mapName)
		{
			return Path.Combine(ROOT, mapName, "World.es3");
		}

		public static string GetIncidentLogPath(string mapName)
		{
			return Path.Combine(ROOT, mapName, "IncidentLog.es3");
		}

		public static string GetAdventureSavePath(string mapName)
		{
			return Path.Combine(ROOT, mapName, "Adventures.es3");
		}

		public static void GetOrCreateMapDirectory(string mapName)
		{
			string[] directoriesAtRoot = Directory.GetDirectories(SaveUtilities.ROOT, mapName);
			if (directoriesAtRoot == null || directoriesAtRoot.Length == 0)
			{
				Directory.CreateDirectory(GetMapRootPath(mapName));
			}
		}

		public static void SerializeSave<T>(T item, string path)
		{
			string output = JsonConvert.SerializeObject(item, Formatting.Indented, SERIALIZER_SETTINGS);
			File.WriteAllText(path, output);
		}

		public static T SerializeLoad<T>(string path, string fileName)
		{
			var files = Directory.GetFiles(path, fileName);
			var file = files[0];

			var text = File.ReadAllText(file);
			if (text.Length > 0)
			{
				return JsonConvert.DeserializeObject<T>(text, SERIALIZER_SETTINGS);
			}

			return default;
		}

		public static T SerializeLoadAll<T>(string path)
		{
			var files = Directory.GetFiles(path, "*.json");
			var file = files[0];

			var text = File.ReadAllText(file);
			if (text.Length > 0)
			{
				return JsonConvert.DeserializeObject<T>(text, SERIALIZER_SETTINGS);
			}

			return default;
		}

		public static void SaveHexMapData(HexGrid grid, string path)
		{
			using (
				BinaryWriter writer =
				new BinaryWriter(File.Open(path, FileMode.Create))
			)
			{
				writer.Write(mapFileVersion);
				grid.Save(writer);
			}
		}

		public static void LoadHexMapData(HexGrid grid, string path)
		{
			if (!File.Exists(path))
			{
				OutputLogger.LogError("File does not exist " + path);
				return;
			}
			using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
			{
				int header = reader.ReadInt32();
				if (header <= mapFileVersion)
				{
					grid.Load(reader, header);
					HexMapCamera.ValidatePosition();
				}
				else
				{
					OutputLogger.LogWarning("Unknown map format " + header);
				}
			}
		}

		public static T ConvertIDToContext<T>(int id)
		{
			return SimulationManager.Instance.AllContexts.GetContextByID<T>(id);
		}

		public static List<T> ConvertIDsToContexts<T>(List<int> ids)
		{
			var contexts = new List<T>();
			foreach (var id in ids)
			{
				contexts.Add(SimulationManager.Instance.AllContexts.GetContextByID<T>(id));
			}
			return contexts;
		}
	}
}