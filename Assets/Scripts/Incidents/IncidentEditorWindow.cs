using Game.IO;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Game.Incidents
{
	public class IncidentEditorWindow : OdinEditorWindow
	{
        [MenuItem("World Builder/Incident Editor")]
		private static void OpenWindow()
		{
			var window = GetWindow<IncidentEditorWindow>("Incident Editor");
            window.minSize = new Vector2(900, 650);
            window.maxSize = new Vector2(1200, 900);
		}

		protected override void OnGUI()
		{
			base.OnGUI();
		}

        private static Type contextType;
        private static Dictionary<string, Type> properties;

        public static Type ContextType
		{
            get { return contextType; }
            set {
                    contextType = value;
                    GetPropertyList();
                }
		}

        static private int numActionFields;
        private bool modeChosen;

        public static Dictionary<string, Type> Properties => properties;
        public static List<IIncidentActionField> actionFields = new List<IIncidentActionField>();

		[ShowIf("@this.modeChosen == true"), ValueDropdown("GetFilteredTypeList"), OnValueChanged("SetContextType"), LabelText("Incident Type"), PropertySpace(SpaceBefore = 30, SpaceAfter = 20)]
        public Type incidentContextType;

        [ShowIfGroup("ContextTypeChosen")]
        public string incidentName;

        [Range(0, 10), ShowIfGroup("ContextTypeChosen")]
        public int weight;

        [ShowIfGroup("ContextTypeChosen"), ListDrawerSettings(CustomAddFunction = "AddNewCriteriaItem"), HideReferenceObjectPicker]
        public List<IIncidentCriteria> criteria;

        [ShowIfGroup("ContextTypeChosen"), HideReferenceObjectPicker, ShowInInspector]
        static public IncidentActionHandlerContainer actionHandler;

        [Button("New Incident"), HorizontalGroup("B1"), PropertyOrder(-1)]
        public void OnNewButtonPressed()
        {
            incidentContextType = null;
            incidentName = string.Empty;
            weight = 0;
            modeChosen = true;
        }

        [Button("Load Incident"), HorizontalGroup("B1"), PropertyOrder(-1)]
        public void OnLoadButtonPressed()
        {
            var dataPath = Path.Combine(Application.dataPath + SaveUtilities.INCIDENT_DATA_PATH + savedIncidentName + ".json");
            var file = File.ReadAllText(dataPath);

            var loadedIncident = JsonConvert.DeserializeObject<Incident>(file, SaveUtilities.SERIALIZER_SETTINGS);
            incidentContextType = loadedIncident.ContextType;
            ContextType = incidentContextType;
            incidentName = savedIncidentName;
            weight = loadedIncident.Weight;
            criteria = loadedIncident.Criteria.criteria;
            actionHandler = loadedIncident.ActionContainer;
            UpdateActionFieldIDs();
            modeChosen = true;

            OutputLogger.Log("Incident Loaded!");
        }

        [ValueDropdown("GetSavedIncidents"), HorizontalGroup("B1")]
        public string savedIncidentName;

        [Button("Save"), ShowIfGroup("ContextTypeChosen"), PropertyOrder(10)]
        public void OnSaveButtonPressed()
		{
            if (ContextTypeChosen && actionHandler.Actions.Count > 0)
            {
                var incident = new Incident(ContextType, criteria, actionHandler, weight);

                var path = Path.Combine(Application.dataPath + SaveUtilities.INCIDENT_DATA_PATH + incidentName + ".json");
                string output = JsonConvert.SerializeObject(incident, Formatting.Indented, SaveUtilities.SERIALIZER_SETTINGS);
                File.WriteAllText(path, output);

                OutputLogger.Log("Incident Saved!");
            }
		}

        public static void UpdateActionFieldIDs()
		{
            actionFields.Clear();
            numActionFields = 0;
            actionHandler.UpdateActionFieldIDs(ref numActionFields);
		}

        private IEnumerable<Type> GetFilteredTypeList()
        {
            var q = typeof(IIncidentContext).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)                                          // Excludes BaseClass
                .Where(x => !x.IsGenericTypeDefinition)                             // Excludes Generics
                .Where(x => typeof(IIncidentContext).IsAssignableFrom(x))           // Excludes classes not inheriting from IIncidentContext
                .Where(x => !typeof(InertIncidentContext).IsAssignableFrom(x))      // Excludes inert contexts
                .Where(x => x.BaseType != typeof(SpecialFaction));                  // Excludes special factions for now

            return q;
        }

        private List<string> GetSavedIncidents()
		{
            var files = Directory.GetFiles(Path.Combine(Application.dataPath + SaveUtilities.INCIDENT_DATA_PATH), "*.json").Select(Path.GetFileNameWithoutExtension).ToList();
            return files;
		}

        void SetContextType()
		{
            ContextType = incidentContextType;
            criteria = new List<IIncidentCriteria>();
            actionHandler = new IncidentActionHandlerContainer(ContextType);
		}

        private void AddNewCriteriaItem()
        {
            criteria.Add(new IncidentCriteria(ContextType));
        }

        private static void GetPropertyList()
        {
            if (ContextType != null)
            {
                var propertyInfo = ContextType.GetProperties();
                var interfacePropertyInfo = typeof(IIncidentContext).GetProperties();

                var validProperties = propertyInfo.Where(x => !interfacePropertyInfo.Any(y => x.Name == y.Name)).ToList();

                if (properties == null)
                {
                    properties = new Dictionary<string, Type>();
                }
                properties.Clear();

                validProperties.ForEach(x => properties.Add(x.Name, x.PropertyType));
            }
        }

        public bool ContextTypeChosen => incidentContextType != null;
    }
}