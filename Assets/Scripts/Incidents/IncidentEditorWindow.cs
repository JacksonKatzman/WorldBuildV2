﻿#if UNITY_EDITOR
using Game.Debug;
using Game.Utilities;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
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

            if(FlavorEditorWindow.Instance == null)
			{
               // FlavorEditorWindow.OpenWindow();
			}
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
            set 
            {
                contextType = value;
                GetPropertyList();
            }
		}

        static private int numActionFields;
        private bool modeChosen;

        public static Dictionary<string, Type> Properties => properties;
        public static List<IIncidentActionField> actionFields = new List<IIncidentActionField>();
        public static List<IContextModifierCalculator> calculators = new List<IContextModifierCalculator>();
        public static List<IncidentActionHandlerContainer> handlerContainers = new List<IncidentActionHandlerContainer>();
        public static Dictionary<int, IIncidentActionField> updates = new Dictionary<int, IIncidentActionField>();
        public static List<int> removedIDs = new List<int>();

		[ShowIf("@this.modeChosen == true"), ValueDropdown("GetFilteredTypeList"), OnValueChanged("SetContextType"), LabelText("Incident Type"), PropertySpace(SpaceBefore = 30, SpaceAfter = 20)]
        public Type incidentContextType;

        [ShowIfGroup("ContextTypeChosen")]
        public string incidentName;

        [ShowIfGroup("ContextTypeChosen"), HideReferenceObjectPicker]
        public IIncidentWeight weight;

        [ShowIfGroup("ContextTypeChosen"), ListDrawerSettings(CustomAddFunction = "AddNewCriteriaItem"), HideReferenceObjectPicker]
        public List<IIncidentCriteria> criteria;

        [ShowIfGroup("ContextTypeChosen"), ListDrawerSettings(CustomAddFunction = "AddNewWorldCriteriaItem"), HideReferenceObjectPicker]
        public List<IIncidentCriteria> worldCriteria;

        [ShowIfGroup("ContextTypeChosen"), HideReferenceObjectPicker, ShowInInspector]
        static public IncidentActionHandlerContainer actionHandler;

        [Button("New Incident"), HorizontalGroup("B1"), PropertyOrder(-1)]
        public void OnNewButtonPressed()
        {
            incidentContextType = null;
            incidentName = string.Empty;
            weight = null;
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
            weight = loadedIncident.Weights;
            criteria = loadedIncident.Criteria.criteria;
            worldCriteria = loadedIncident.WorldCriteria == null ? new List<IIncidentCriteria>() : loadedIncident.WorldCriteria.criteria;
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
            if (ContextTypeChosen && !string.IsNullOrEmpty(incidentName))// && actionHandler.Actions.Count > 0)
            {
                var incident = new Incident(incidentName, ContextType, criteria, worldCriteria, actionHandler, weight);

                var path = Path.Combine(Application.dataPath + SaveUtilities.INCIDENT_DATA_PATH + incidentName + ".json");
                string output = JsonConvert.SerializeObject(incident, Formatting.Indented, SaveUtilities.SERIALIZER_SETTINGS);
                File.WriteAllText(path, output);

                AssetDatabase.Refresh();
                OutputLogger.Log("Incident Saved!");
            }
		}

        public static void UpdateActionFieldIDs()
		{
            updates.Clear();
            handlerContainers.Clear();
            actionFields.Clear();
            calculators.Clear();
            numActionFields = 0;
            UpdateMainContextActionFieldIDs(ref numActionFields);
            actionHandler.UpdateActionFieldIDs(ref numActionFields);
            CompleteLogIDUpdate();
		}

        public static void UpdateLogIDs(int oldID, IIncidentActionField actionField)
		{
            if (oldID > -1 && !updates.ContainsKey(oldID))
            {
                updates.Add(oldID, actionField);
            }
		}

        private static void CompleteLogIDUpdate()
		{
            foreach(var field in actionFields)
			{
                var prev = field.PreviousFieldID;
                if(removedIDs.Contains(prev))
				{
                    field.PreviousFieldID = -1;
                    field.PreviousField = "REMOVED";
                }
                if(updates.ContainsKey(prev))
				{
                    field.PreviousFieldID = updates[prev].ActionFieldID;
                    field.PreviousField = updates[prev].NameID;
				}
			}
            
            foreach (var container in handlerContainers)
            {
                if (container.incidentLog != null)
                {
                    foreach (var pair in updates)
                    {
                        container.incidentLog = Regex.Replace(container.incidentLog, @"\{" + pair.Key + @"\}", @"{replace" + pair.Value.ActionFieldID + @"}");
                        container.UpdatedDeployableContextIDs(updates);
                    }
                    container.incidentLog = Regex.Replace(container.incidentLog, @"{replace", @"{");
                }
            }

            removedIDs.Clear();
        }

		private static void UpdateMainContextActionFieldIDs(ref int startingValue)
		{
            if (startingValue == 0)
            {
                var constant = new ConstantActionField(ContextType);
                actionFields.Add(constant);
                startingValue++;
            }

            var matchingFields = ActionFieldReflection.GetGenericFieldsByType(ContextType, typeof(DeployedContextActionField<>));
            foreach (var f in matchingFields)
            {
                var dataType = new Type[] { f.FieldType.GetGenericArguments()[0] };
                var genericBase = typeof(DeployedContextActionField<>);
                var combinedType = genericBase.MakeGenericType(dataType);
                var actionField = (IIncidentActionField)Activator.CreateInstance(combinedType, ContextType);
                actionField.ActionFieldID = startingValue;
                actionField.NameID = string.Format("{0}:{1}:{2}", actionField.ActionFieldIDString, ContextType.Name, f.Name);
                actionFields.Add(actionField);
                startingValue++;
            }
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

        private void SetContextType()
		{
            ContextType = incidentContextType;
            criteria = new List<IIncidentCriteria>();
            worldCriteria = new List<IIncidentCriteria>();
            actionHandler = new IncidentActionHandlerContainer(ContextType);
            CreateIncidentWeight();
		}

        private void CreateIncidentWeight()
		{
            var dataType = new Type[] { incidentContextType };
            var genericBase = typeof(IncidentWeight<>);
            var combinedType = genericBase.MakeGenericType(dataType);
            weight = (IIncidentWeight)Activator.CreateInstance(combinedType);
        }

        private void AddNewCriteriaItem()
        {
            criteria.Add(new IncidentCriteria(ContextType));
        }

        private void AddNewWorldCriteriaItem()
        {
            worldCriteria.Add(new WorldCriteria());
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
#endif