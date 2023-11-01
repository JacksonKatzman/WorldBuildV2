using Game.Enums;
using Game.Generators.Items;
using Game.Incidents;
using Game.Simulation;
using Game.Utilities;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class Landmark : InertIncidentContext, ILocationAffiliated, IInventoryAffiliated, IOrganizationAffiliated
	{
		public Location CurrentLocation { get; set; }

		public override Type ContextType => typeof(Landmark);

		public Inventory CurrentInventory { get; set; }
		public Organization AffiliatedOrganization { get; set; }

		public LandmarkPreset Preset { get; private set; }

		public Landmark() 
		{
			EventManager.Instance.AddEventHandler<RemoveContextEvent>(OnRemoveContextEvent);
		}
		public Landmark(Location location, LandmarkPreset landmarkType) : this()
		{
			CurrentLocation = location;
			CurrentInventory = new Inventory();
			Preset = landmarkType;
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
			base.Die();
		}

		private void OnRemoveContextEvent(RemoveContextEvent gameEvent)
		{
			if(gameEvent.context == AffiliatedOrganization)
			{
				AffiliatedOrganization = null;
			}
		}
	}
}