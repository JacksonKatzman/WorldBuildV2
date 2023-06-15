using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using Game.Simulation;
using Game.Utilities;

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

		public SimulationManager SimulationManager => SimulationManager.Instance;

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
			string path = SaveUtilities.GetMapRootPath(nameInput.text);

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
				Directory.CreateDirectory(SaveUtilities.ROOT);
			}
			string[] paths =
				Directory.GetDirectories(SaveUtilities.ROOT);
			Array.Sort(paths);
			for (int i = 0; i < paths.Length; i++)
			{
				SaveLoadItem item = Instantiate(itemPrefab);
				item.menu = this;
				item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
				item.transform.SetParent(listContent, false);
			}
		}

		void Save(string mapName)
		{
			SimulationManager.SaveWorld(mapName);
		}

		void Load(string mapName)
		{
			SimulationManager.LoadWorld(mapName);
		}
	}
}