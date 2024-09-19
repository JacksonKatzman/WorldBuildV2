#if UNITY_EDITOR
using Game.Utilities;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Game.Incidents
{
    public class IncidentJSONToolWindow : OdinEditorWindow
	{
        [MenuItem("World Builder/Incident JSON Tools")]
        private static void OpenWindow()
        {
            var window = GetWindow<IncidentJSONToolWindow>("Incident JSON Tools");
            window.minSize = new Vector2(900, 650);
            window.maxSize = new Vector2(1200, 900);
        }

        public string toReplace;
        public string replaceWith;

        [Button("Replace Instances")]
        private void ReplaceInstances()
		{
            string[] files = Directory.GetFiles(Application.dataPath + SaveUtilities.INCIDENT_DATA_PATH);
            foreach(var file in files)
			{
                var text = File.ReadAllText(file);
                text = text.Replace(toReplace, replaceWith);
                File.WriteAllText(file, text);
			}
		}
    }
}
#endif