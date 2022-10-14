using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Game.Simulation;

namespace Game.Terrain
{
	public class SaveLoadMenu : MonoBehaviour
	{

		const int mapFileVersion = 4;

		public Text menuLabel, actionButtonLabel;

		public InputField nameInput;

		public RectTransform listContent;

		public SaveLoadItem itemPrefab;

		//public HexGrid hexGrid;

		public SimulationManager simulationManager;

		bool saveMode;

		public void Open(bool saveMode)
		{
			this.saveMode = saveMode;
			if (saveMode)
			{
				menuLabel.text = "Save Map";
				actionButtonLabel.text = "Save";
			}
			else
			{
				menuLabel.text = "Load Map";
				actionButtonLabel.text = "Load";
			}
			FillList();
			gameObject.SetActive(true);
			HexMapCamera.Locked = true;
		}

		public void Close()
		{
			gameObject.SetActive(false);
			HexMapCamera.Locked = false;
		}

		public void Action()
		{
			var mapName = nameInput.text;

			if (mapName.Length == 0)
			{
				return;
			}
			if (saveMode)
			{
				Save(mapName);
			}
			else
			{
				Load(mapName);
			}
			Close();
		}

		public void SelectItem(string name)
		{
			nameInput.text = name;
		}

		public void Delete()
		{
			string path = WorldSaveDirectories.GetMapRootPath(nameInput.text);

			if (path == null)
			{
				return;
			}
			if (Directory.Exists(path))
			{
				Directory.Delete(path, true);
			}
			nameInput.text = "";
			FillList();
		}

		void FillList()
		{
			for (int i = 0; i < listContent.childCount; i++)
			{
				Destroy(listContent.GetChild(i).gameObject);
			}
			string[] directoriesAtRoot = Directory.GetDirectories(Application.persistentDataPath, "GameData");
			if(directoriesAtRoot == null || directoriesAtRoot.Length == 0)
			{
				Directory.CreateDirectory(WorldSaveDirectories.ROOT);
			}
			string[] paths =
				Directory.GetDirectories(WorldSaveDirectories.ROOT);
			Array.Sort(paths);
			for (int i = 0; i < paths.Length; i++)
			{
				SaveLoadItem item = Instantiate(itemPrefab);
				item.menu = this;
				item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
				item.transform.SetParent(listContent, false);
			}
		}

		private void CreateMapDirectories(string mapName)
		{
			Directory.CreateDirectory(WorldSaveDirectories.GetHexMapDataPath(mapName));
		}

		void Save(string mapName)
		{
			string[] directoriesAtRoot = Directory.GetDirectories(WorldSaveDirectories.ROOT, mapName);
			if (directoriesAtRoot == null || directoriesAtRoot.Length == 0)
			{
				CreateMapDirectories(mapName);
			}
			SaveHexMapData(WorldSaveDirectories.GetHexMapData(mapName));
		}

		void Load(string mapName)
		{
			LoadHexMapData(WorldSaveDirectories.GetHexMapData(mapName));
		}

		void SaveHexMapData(string path)
		{
			using (
				BinaryWriter writer =
				new BinaryWriter(File.Open(path, FileMode.Create))
			)
			{
				writer.Write(mapFileVersion);
				simulationManager.hexGrid.Save(writer);
			}
		}

		void LoadHexMapData(string path)
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
					simulationManager.hexGrid.Load(reader, header);
					HexMapCamera.ValidatePosition();
				}
				else
				{
					OutputLogger.LogWarning("Unknown map format " + header);
				}
			}
		}
	}

	public static class WorldSaveDirectories
	{
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
			return Path.Combine(ROOT, mapName, "HexMapData", mapName + ".map");
		}
	}
}