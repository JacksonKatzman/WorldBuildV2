using Game.Debug;
using Game.Enums;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public class GetFlavorAction : GenericIncidentAction
	{
		public int FlavorActionId { get; set; }

		[ShowInInspector, ReadOnly]
		public string FlavorActionIdString => "{F:" + FlavorActionId + "}";

		[ValueDropdown("GetFilteredFlavorTypeList"), OnValueChanged("SetFlavorType"), LabelText("Flavor Type"), PropertySpace(SpaceAfter = 20)]
		public Type flavorType;

		[OnValueChanged("ToggleManualMode")]
		public bool manualMode;

		[ShowIf("@this.manualMode == false")]
		public InterfacedIncidentActionFieldContainer<IAlignmentAffiliated> alignmentContext;

		[ShowIf("@this.manualMode == true")]
		public OrganizationType perm;

		[ShowIf("@this.manualMode == true"), PropertyRange(-10, 10)]
		public int goodEvilAxisAlignment;

		[ShowIf("@this.manualMode == true"), PropertyRange(-10, 10)]
		public int lawfulChaoticAxisAlignment;

		public List<IncidentActionFieldContainer> actionFields;

		[HideInInspector]
		public Dictionary<string, IncidentActionFieldContainer> pairings;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			if (manualMode)
			{
				AddFlavor(flavorType, perm, goodEvilAxisAlignment, lawfulChaoticAxisAlignment, ref report);
			}
			else
			{
				var cont = alignmentContext.GetTypedFieldValue();
				AddFlavor(flavorType, cont.PriorityAlignment, cont.GoodEvilAlignmentAxis, cont.LawfulChaoticAlignmentAxis, ref report);
			}
		}

		private void AddFlavor(Type type, OrganizationType priority, int goodEvilAxisAlignment, int lawfulChaoticAxisAlignment, ref IncidentReport report)
		{
			string templateString = "";
			if (FlavorService.Instance.GetFlavorTemplateByType(type, priority, goodEvilAxisAlignment, lawfulChaoticAxisAlignment, out var template))
			{
				templateString = string.Copy(template.flavor);
				foreach (var pair in pairings)
				{
					templateString = templateString.Replace(pair.Key, "[" + pair.Value.actionField.ActionFieldID + "]");
				}

				templateString = templateString.Replace('[', '{');
				templateString = templateString.Replace(']', '}');

				report.AddFlavor(FlavorActionIdString, templateString);
			}
			else
			{
				OutputLogger.LogError("Matching flavor template missing!");
			}
		}
#if UNITY_EDITOR
		private IEnumerable<Type> GetFilteredFlavorTypeList()
		{
			var q = typeof(IFlavorTemplate).Assembly.GetTypes()
				.Where(x => !x.IsAbstract)                                          // Excludes BaseClass
				.Where(x => !x.IsGenericTypeDefinition)                             // Excludes Generics
				.Where(x => typeof(IFlavorTemplate).IsAssignableFrom(x));
			return q;
		}

		private void SetFlavorType()
		{
			OutputLogger.Log("Set Flavor Type");

			actionFields.Clear();
			IncidentEditorWindow.UpdateActionFieldIDs();

			if(pairings == null)
			{
				pairings = new Dictionary<string, IncidentActionFieldContainer>();
			}
			else
			{
				pairings.Clear();
			}

			var contextFields = flavorType.GetFields().Where(x => x.FieldType.IsGenericType &&  x.FieldType.GetGenericTypeDefinition() == typeof(IndexedObject<>) && typeof(IIncidentContext).IsAssignableFrom(x.FieldType.GetGenericArguments().First())).ToList();
			for(var i = 0; i < contextFields.Count; i++)
			{
				var fieldInfo = contextFields[i];
				var actionFieldContainer = new IncidentActionFieldContainer();
				actionFieldContainer.ForceSetContextType(fieldInfo.FieldType.GetGenericArguments().First(), fieldInfo.Name);
				actionFields.Add(actionFieldContainer);

				var pairingKey = "{" + i + "}";
				pairings.Add(pairingKey, actionFieldContainer);
			}

			IncidentEditorWindow.UpdateActionFieldIDs();
		}

		private void ToggleManualMode()
		{
			alignmentContext.enabled = !manualMode;
		}
#endif
	}
}