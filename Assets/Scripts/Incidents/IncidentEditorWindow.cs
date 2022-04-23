using System.Collections;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace Game.Incidents
{
	public class IncidentEditorWindow : OdinEditorWindow
	{
        public static string INCIDENT_DATA_PATH = "/Resources/IncidentData.json";
        public static JsonSerializerSettings SERIALIZER_SETTINGS = new JsonSerializerSettings()
        {
            // ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            PreserveReferencesHandling = PreserveReferencesHandling.Objects,
            TypeNameHandling = TypeNameHandling.All
        };

        public EditableCoreIncident container;
        //private Editor editor;
        private OdinEditor editor;
        
        [MenuItem("Window/Incident Editor")]
        public static void ShowWindow()
        {
            GetWindow<IncidentEditorWindow>("Incident Editor");
        }

        void OnGUI()
        {
            if (GUILayout.Button("New Incident"))
            {
                container = CreateInstance<EditableCoreIncident>();
                editor = OdinEditor.CreateEditor(container, typeof(OdinEditor)) as OdinEditor;
            }
            //container = EditorGUILayout.ObjectField(container, typeof(EditableCoreIncident)) as EditableCoreIncident;
            editor?.DrawDefaultInspector();

            if(GUILayout.Button("Save Incident"))
            {
                SerializeIncident(container);
            }
        }

        private void SerializeIncident(EditableCoreIncident incidentData)
		{
            var incidents = ParseIncidents();

            var coreIncident = new CoreIncident(incidentData);

            if (!incidents.Any(x => x.incidentName == coreIncident.incidentName))
            {
                incidents.Add(coreIncident);

                var settings = new JsonSerializerSettings()
                {
                    // ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                    TypeNameHandling = TypeNameHandling.All
                };

                string output = JsonConvert.SerializeObject(incidents, Formatting.Indented, SERIALIZER_SETTINGS);

                File.WriteAllText(Application.dataPath + INCIDENT_DATA_PATH, output);
            }
            else
			{
                OutputLogger.LogError("Incident with that name already exists!");
			}
		}

        public static List<CoreIncident> ParseIncidents()
		{
            var list = new List<CoreIncident>();
            var text = File.ReadAllText(Application.dataPath + INCIDENT_DATA_PATH);
            if (text.Length > 0)
            {
                list = JsonConvert.DeserializeObject<List<CoreIncident>>(text, SERIALIZER_SETTINGS);
            }
            return list;
		}
    }

    //[CreateAssetMenu(fileName = nameof(EditableCoreIncident), menuName = "ScriptableObjects/Incidents/" + nameof(EditableCoreIncident), order = 1)]
    public class EditableCoreIncident : SerializedScriptableObject
    {
        public string incidentName;
        public List<IIncidentTag> tags;
        public int weight;

        public List<IncidentModifier> required;
        public List<IncidentModifier> optional;
    }
}