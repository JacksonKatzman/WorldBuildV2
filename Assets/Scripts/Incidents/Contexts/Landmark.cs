﻿using Game.Generators.Items;
using Game.Incidents;
using Game.Utilities;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class Landmark : InertIncidentContext, ILocationAffiliated, IInventoryAffiliated
	{
		public Location CurrentLocation { get; set; }

		public override Type ContextType => typeof(Landmark);

		public Inventory CurrentInventory { get; set; }

		public Landmark() { }
		public Landmark(Location location)
		{
			CurrentLocation = location;
			CurrentInventory = new Inventory();
		}

		public override void LoadContextProperties()
		{
			CurrentLocation = SaveUtilities.ConvertIDToContext<Location>(contextIDLoadBuffers["CurrentLocation"][0]);
			CurrentInventory.LoadContextProperties();

			contextIDLoadBuffers.Clear();
		}
	}
}