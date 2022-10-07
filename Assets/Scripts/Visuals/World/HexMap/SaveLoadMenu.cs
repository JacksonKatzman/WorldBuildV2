using System;
using System.IO;
using TMPro;
using UnityEngine;

namespace Game.Visuals.Hex
{
	public class SaveLoadMenu : MonoBehaviour
	{
		public const int SAVE_VERSION = 3;

		public HexGrid hexGrid;

		public TMP_Text menuLabel, actionButtonLabel;

		public TMP_InputField nameInput;

		public RectTransform listContent;

		public SaveLoadItem itemPrefab;

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
			string path = GetSelectedPath();
			if (path == null)
			{
				return;
			}
			if (saveMode)
			{
				Save(path);
			}
			else
			{
				Load(path);
			}
			Close();
		}

		public void SelectItem(string name)
		{
			nameInput.text = name;
		}

		public void Delete()
		{
			string path = GetSelectedPath();
			if (path == null)
			{
				return;
			}

			if (File.Exists(path))
			{
				File.Delete(path);
			}

			nameInput.text = "";
			FillList();
		}

		string GetSelectedPath()
		{
			string mapName = nameInput.text;
			if (mapName.Length == 0)
			{
				return null;
			}
			return Path.Combine(Application.persistentDataPath, mapName + ".map");
		}

		void Save(string path)
		{
			using (BinaryWriter writer = new BinaryWriter(File.Open(path, FileMode.Create)))
			{
				writer.Write(SAVE_VERSION);
				hexGrid.Save(writer);
			}
		}

		void Load(string path)
		{
			if (!File.Exists(path))
			{
				OutputLogger.LogError("File does not exist " + path);
				return;
			}
			using (BinaryReader reader = new BinaryReader(File.OpenRead(path)))
			{
				int header = reader.ReadInt32();
				if (header <= SAVE_VERSION)
				{
					hexGrid.Load(reader, header);
					HexMapCamera.ValidatePosition();
				}
				else
				{
					OutputLogger.LogError("Unknown map format " + header + ". For safety, will not Load.");
				}
			}
		}

		void FillList()
		{
			for (int i = 0; i < listContent.childCount; i++)
			{
				Destroy(listContent.GetChild(i).gameObject);
			}

			string[] paths =
				Directory.GetFiles(Application.persistentDataPath, "*.map");
			Array.Sort(paths);

			for (int i = 0; i < paths.Length; i++)
			{
				SaveLoadItem item = Instantiate(itemPrefab);
				item.menu = this;
				item.MapName = Path.GetFileNameWithoutExtension(paths[i]);
				item.transform.SetParent(listContent, false);
			}
		}
	}
}
