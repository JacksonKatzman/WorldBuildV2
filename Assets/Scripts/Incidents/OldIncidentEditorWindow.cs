#if UNITY_EDITOR
using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using Sirenix.OdinInspector;

namespace Game.Incidents
{
	public class OldIncidentEditorWindow : OdinEditorWindow
	{
        public static string INCIDENT_DATA_PATH = "/Resources/IncidentData/";
        public static JsonSerializerSettings SERIALIZER_SETTINGS = new JsonSerializerSettings()
        {
            // ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.All
        };

        public EditableIncident container;
        //private Editor editor;
        private OdinEditor editor;
        
        [MenuItem("Window/Old Incident Editor")]
        public static void ShowWindow()
        {
            GetWindow<OldIncidentEditorWindow>("Incident Editor");
        }

        void OnGUI()
        {
            if (GUILayout.Button("New Incident"))
            {
                container = CreateInstance<EditableIncident>();
                editor = OdinEditor.CreateEditor(container, typeof(OdinEditor)) as OdinEditor;
            }
            //container = EditorGUILayout.ObjectField(container, typeof(EditableCoreIncident)) as EditableCoreIncident;
            editor?.DrawDefaultInspector();

            if(GUILayout.Button("Save Incident"))
            {
                //SerializeIncident(container);
            }
        }

        private void SerializeIncident(EditableCoreIncident incidentData)
		{
            var incidents = ParseIncidents();

            var coreIncident = new CoreIncident(incidentData);

            if (!incidents.Any(x => x.incidentName == coreIncident.incidentName))
            {
                incidents.Add(coreIncident);

                string output = JsonConvert.SerializeObject(incidents, Formatting.Indented, SERIALIZER_SETTINGS);

                File.WriteAllText(Application.dataPath + INCIDENT_DATA_PATH + coreIncident.incidentName + ".json", output);
            }
            else
			{
                OutputLogger.LogError("Incident with that name already exists!");
			}
		}

        public static List<CoreIncident> ParseIncidents()
		{
            var list = new List<CoreIncident>();
            var files = Directory.GetFiles(Application.dataPath + INCIDENT_DATA_PATH, "*.json");
            foreach(string file in files)
			{
                var text = File.ReadAllText(file);
                if (text.Length > 0)
                {
                    list = JsonConvert.DeserializeObject<List<CoreIncident>>(text, SERIALIZER_SETTINGS);
                }
            }
            return list;
        }
    }

    public class EditableIncident : SerializedScriptableObject
    {
        public string incidentName;
        public int weight;
    }
}
#endif