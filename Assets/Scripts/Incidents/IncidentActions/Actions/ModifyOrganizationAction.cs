using Game.Debug;
using Game.Simulation;
using Game.Utilities;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class ModifyOrganizationAction : GenericIncidentAction
	{
		public enum OrganizationModificationAction { REPLACE, REMOVE }
		public InterfacedIncidentActionFieldContainer<IOrganizationAffiliated> organization;

		[OnValueChanged("OnModActionChanged")]
		public OrganizationModificationAction modAction;

		[ShowIf("modAction", OrganizationModificationAction.REPLACE), ListDrawerSettings(CustomAddFunction = "AddScriptableObjectRetriever")]
		public List<ScriptableObjectRetriever<OrganizationTemplate>> possibleTemplates;

		[ShowIf("modAction", OrganizationModificationAction.REPLACE)]
		public InterfacedIncidentActionFieldContainer<ISentient> creator;

		public override void PerformAction(IIncidentContext context, ref IncidentReport report)
		{
			if(modAction == OrganizationModificationAction.REPLACE)
			{
				ReplaceOrganization();
			}
			else if(modAction == OrganizationModificationAction.REMOVE)
			{
				RemoveOrganization();
			}
		}

		private void ReplaceOrganization()
		{
			var template = SimRandom.RandomEntryFromList(possibleTemplates).RetrieveObject();

			var org = organization.GetTypedFieldValue().AffiliatedOrganization;
			var faction = org.AffiliatedFaction;

			var newOrg = new Organization(template, faction, faction.AffiliatedRace, creator.GetTypedFieldValue());

			if (faction != null && org == faction.Government)
			{
				org.Die();
				faction.Government = newOrg;
			}
			else
			{
				Landmark landmark = ContextDictionaryProvider.CurrentContexts[typeof(Landmark)].First(x => ((Landmark)x).AffiliatedOrganization == org) as Landmark;
				if(landmark != null)
				{
					org.Die();
					landmark.AffiliatedOrganization = newOrg;
				}
			}

			EventManager.Instance.Dispatch(new AddContextEvent(newOrg, false));
		}

		private void RemoveOrganization()
		{
			var org = organization.GetTypedFieldValue().AffiliatedOrganization;
			var faction = org.AffiliatedFaction;

			if (faction != null && org == faction.Government)
			{
				OutputLogger.LogWarning("Something tried to REMOVE a Faction's Government.");
			}
			else
			{
				Landmark landmark = ContextDictionaryProvider.CurrentContexts[typeof(Landmark)].First(x => ((Landmark)x).AffiliatedOrganization == org) as Landmark;
				if (landmark != null)
				{
					org.Die();
				}
			}
		}

		private void AddScriptableObjectRetriever()
		{
			possibleTemplates.Add(new ScriptableObjectRetriever<OrganizationTemplate>());
		}

		private void OnModActionChanged()
		{
			creator.enabled = modAction == OrganizationModificationAction.REPLACE ? true : false;
		}
	}
}