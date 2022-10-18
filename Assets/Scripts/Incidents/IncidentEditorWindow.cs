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

        private int numActionFields;

        public static Dictionary<string, Type> Properties => properties;
        public static List<IIncidentActionField> actionFields = new List<IIncidentActionField>();

		[TypeFilter("GetFilteredTypeList"), OnValueChanged("SetContextType"), LabelText("Incident Type")]
        public IIncidentContext incidentContext;

        [ShowIfGroup("ContextTypeChosen")]
        public string incidentName;

        [Range(0, 10), ShowIfGroup("ContextTypeChosen")]
        public int weight;

        [ShowIfGroup("ContextTypeChosen"), ListDrawerSettings(CustomAddFunction = "AddNewCriteriaItem"), HideReferenceObjectPicker]
        public List<IIncidentCriteria> criteria;

        [ShowIfGroup("ContextTypeChosen")]
        public string incidentLog;

        [ShowIfGroup("ContextTypeChosen"), ListDrawerSettings(CustomAddFunction = "AddNewActionItem"), HideReferenceObjectPicker]
        public List<ActionChoiceContainer> actions;

        [Button("Save"), ShowIfGroup("ContextTypeChosen")]
        public void OnSaveButtonPressed()
		{
            if (ContextTypeChosen && actions.Count > 0)
            {
                var incidentActions = new List<IIncidentAction>();
                actions.ForEach(x => incidentActions.Add(x.incidentAction));
                var container = new IncidentActionContainer(incidentActions);
                container.incidentLog = incidentLog;

                var incident = new Incident(ContextType, criteria, container, weight);

                //Save this data somewhere T.T
                var path = Path.Combine(Application.dataPath + SaveUtilities.INCIDENT_DATA_PATH + incidentName + ".json");
                string output = JsonConvert.SerializeObject(incident, Formatting.Indented, SaveUtilities.SERIALIZER_SETTINGS);
                File.WriteAllText(path, output);
            }
		}

        private IEnumerable<Type> GetFilteredTypeList()
        {
            var q = typeof(IIncidentContext).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)                                          // Excludes BaseClass
                .Where(x => !x.IsGenericTypeDefinition)                             // Excludes C1<>
                .Where(x => typeof(IIncidentContext).IsAssignableFrom(x));          // Excludes classes not inheriting from BaseClass

            return q;
        }

        void SetContextType()
		{
            ContextType = incidentContext.ContextType;
            criteria = new List<IIncidentCriteria>();
            actions = new List<ActionChoiceContainer>();
		}

        private void AddNewCriteriaItem()
        {
            criteria.Add(new IncidentCriteria(ContextType));
        }

        private void AddNewActionItem()
		{
            actions.Add(new ActionChoiceContainer(UpdateActionFieldIDs));
		}

        public void UpdateActionFieldIDs()
		{
            var fieldCount = 0;
            actionFields.Clear();

            foreach (var a in actions)
			{
                var incidentAction = a.incidentAction;
                var actionType = incidentAction.GetType();
                var fields = actionType.GetFields();
                var matchingFields = fields.Where(x => x.FieldType.IsGenericType && x.FieldType.GetGenericTypeDefinition() == typeof(IncidentContextActionField<>));

                foreach(var f in matchingFields)
				{
                    var fa = f.GetValue(incidentAction) as IIncidentActionField;
                    fa.ActionFieldID = fieldCount;
                    fa.NameID = string.Format("{0}:{1}:{2}", fa.ActionFieldIDString, actionType.Name, f.Name);
                    actionFields.Add(fa);
                    fieldCount++;
				}
			}
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

        public bool ContextTypeChosen => incidentContext != null;
    }

    public class ActionChoiceContainer
	{
        [TypeFilter("GetFilteredTypeList"), OnValueChanged("SetAction"), HideLabel]
        public IIncidentAction incidentAction;

        private Action onSetCallback;

        public ActionChoiceContainer() { }
        public ActionChoiceContainer(Action onSetCallback)
		{
			this.onSetCallback = onSetCallback;
		}

		public IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly assembly)
        {
            return from x in assembly.GetTypes()
                   from z in x.GetInterfaces()
                   let y = x.BaseType
                   where
                   (y != null && y.IsGenericType &&
                   openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
                   (z.IsGenericType &&
                   openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
                   select x;
        }

        public IEnumerable<Type> GetFilteredTypeList()
		{
            var allActions = GetAllTypesImplementingOpenGenericType(typeof(IIncidentAction), Assembly.GetExecutingAssembly());
            var matches = allActions.Where(x => x.BaseType.IsGenericType == true && x.BaseType.GetGenericArguments()[0] == IncidentEditorWindow.ContextType).ToList();
            return matches;
		}

        public void SetAction()
		{
            incidentAction.UpdateEditor();
            onSetCallback?.Invoke();
        }
    }
}