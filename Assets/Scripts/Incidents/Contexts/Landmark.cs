using Game.Enums;
using Game.Generators.Items;
using Game.Incidents;
using Game.Simulation;
using Game.Utilities;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class Landmark : InertIncidentContext, ILocationAffiliated, IInventoryAffiliated, IOrganizationAffiliated, IFactionAffiliated
	{
		public Location CurrentLocation { get; set; }

		public override Type ContextType => typeof(Landmark);

		public Inventory CurrentInventory { get; set; }
		public Faction AffiliatedFaction { get; set; }
		public Organization AffiliatedOrganization { get; set; }
		public LandmarkPreset Preset { get; private set; }
		public List<LandmarkTag> LandmarkTags { get; set; }

		public Landmark() 
		{
			EventManager.Instance.AddEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
			EventManager.Instance.AddEventHandler<TerritoryChangedControlEvent>(OnTerritoryChangedControl);
		}
		public Landmark(Location location, LandmarkPreset landmarkType) : this()
		{
			CurrentLocation = location;
			CurrentInventory = new Inventory();
			Preset = landmarkType;
			LandmarkTags = Preset.landmarkTags != null ? Preset.landmarkTags : new List<LandmarkTag>();
			Name = Preset.name;
		}

		public override void LoadContextProperties()
		{
			CurrentLocation = SaveUtilities.ConvertIDToContext<Location>(contextIDLoadBuffers["CurrentLocation"][0]);
			CurrentInventory.LoadContextProperties();

			contextIDLoadBuffers.Clear();
		}

		public override void Die()
		{
			EventManager.Instance.RemoveEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
			EventManager.Instance.RemoveEventHandler<TerritoryChangedControlEvent>(OnTerritoryChangedControl);
			base.Die();
		}

		private void OnRemoveContextEvent(RemoveContextEvent gameEvent)
		{
			if(gameEvent.context == AffiliatedOrganization)
			{
				AffiliatedOrganization = null;
			}
		}

		private void OnTerritoryChangedControl(TerritoryChangedControlEvent gameEvent)
		{
			if(CurrentLocation.Equals(gameEvent.location.CurrentLocation))
			{
				AffiliatedFaction = gameEvent.territoryGainer.AffiliatedFaction;
				if(AffiliatedOrganization != null)
				{
					var orgTemplate = AffiliatedOrganization.template;
					var gainer = gameEvent.territoryGainer.AffiliatedFaction;
					AffiliatedOrganization.Die();
					AffiliatedOrganization = new Organization(orgTemplate, gainer, gainer.AffiliatedRace);
				}
			}
		}
	}
}