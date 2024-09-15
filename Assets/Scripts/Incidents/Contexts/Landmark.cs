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
		public List<LandmarkTrait> LandmarkTraits { get; set; }

        public override string Description => $"LANDMARK DESCRIPTION";

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
			LandmarkTraits = Preset.landmarkTraits != null ? Preset.landmarkTraits : new List<LandmarkTrait>();
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