using Game.Enums;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class GetFlavorAction : GenericIncidentAction
	{
		public int FlavorActionId { get; set; }

		[ShowInInspector, ReadOnly]
		public string FlavorActionIdString => "{F:" + FlavorActionId + "}";

		[ValueDropdown("GetFilteredFlavorTypeList"), OnValueChanged("SetFlavorType"), LabelText("Flavor Type"), PropertySpace(SpaceAfter = 20)]
		public Type flavorType;

		public bool manualMode;

		[ShowIf("@this.manualMode == false")]
		public InterfacedIncidentActionFieldContainer<IAlignmentAffiliated> alignmentContext;

		[ShowIf("@this.manualMode == true")]
		public OrganizationType perm;

		[ShowIf("@this.manualMode == true"), PropertyRange(-10,10)]
		public int goodEvilAxisAlignment;

		[ShowIf("@this.manualMode == true"), PropertyRange(-10, 10)]
		public int lawfulChaoticAxisAlignment;

		public List<IncidentActionFieldContainer> actionFields;
		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			//need to grab the flavor from somewhere and add it to the report, perhaps as a list of flavors that lives in the report
			//also need to asign a flavor id to the flavor grabbed and ensure its unique and updates when you add or remove GetFlavorActions
			IFlavorTemplate template;
			if (manualMode)
			{
				template = FlavorService.Instance.GetFlavorTemplateByType(flavorType, perm, goodEvilAxisAlignment, lawfulChaoticAxisAlignment);
			}
			else
			{
				var cont = alignmentContext.GetTypedFieldValue();
				template = FlavorService.Instance.GetFlavorTemplateByType(flavorType, cont.PriorityAlignment, cont.GoodEvilAlignmentAxis, cont.LawfulChaoticAlignmentAxis);
			}
		}

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

			//var contextFields = flavorType.GetFields().Where(x => typeof(IIncidentContext).IsAssignableFrom(x.FieldType)).ToList();
			var test = flavorType.GetFields().ToList();
			var contextFields = flavorType.GetFields().Where(x => x.FieldType.GetGenericTypeDefinition() == typeof(IndexedObject<>) && typeof(IIncidentContext).IsAssignableFrom(x.FieldType.GetGenericArguments().First()));
			foreach(var fieldInfo in contextFields)
			{
				var actionFieldContainer = new IncidentActionFieldContainer();
				actionFieldContainer.ForceSetContextType(fieldInfo.FieldType.GetGenericArguments().First(), fieldInfo.Name);
				actionFields.Add(actionFieldContainer);
			}

			IncidentEditorWindow.UpdateActionFieldIDs();
			/*
			var dataType = new Type[] { incidentContextType };
			var genericBase = typeof(IncidentWeight<>);
			var combinedType = genericBase.MakeGenericType(dataType);
			weight = (IIncidentWeight)Activator.CreateInstance(combinedType);
			*/
		}
	}
}