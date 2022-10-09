using Game.Data.EventHandling.EventRecording;
using Game.Generators;
using Game.Generators.Items;
using Game.WorldGeneration;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine.Events;

namespace Game.Incidents
{
	public class CreateLandmarkModifier : IncidentModifier, ITileLocationContainer, IInventoryContainer
	{
		[ValueDropdown("GrabInterfaceTypes")]
		public Type landmarkType;

		private List<Tile> locations;
		private List<Item> inventory;

		public List<Item> Inventory => inventory;

		public List<Tile> Locations => locations;


		public CreateLandmarkModifier(List<IIncidentTag> tags, float probability) : base(tags, probability)
		{
			inventory = new List<Item>();
			locations = new List<Tile>();
		}

		public static IEnumerable<Type> GrabInterfaceTypes()
		{
			return IncidentHelpers.GetInterfaceTypes(typeof(ILandmark));
		}

		public override void Run(OldIncidentContext context)
		{
			base.Run(context);
			foreach (var location in locations)
			{
				var landmark = System.Activator.CreateInstance(landmarkType);
				if (typeof(Landmark).IsAssignableFrom(landmark.GetType()))
				{
					LandmarkGenerator.RegisterLandmark(location, (Landmark)landmark);
					(landmark as IInventoryContainer)?.Inventory.AddRange(inventory);
				}
			}
		}

		public override void LogModifier()
		{
			var landmarkName = (landmarkType as IRecordable).Name;

			foreach(var location in locations)
			{
				incidentLogs.Add(landmarkName + " was created within " + location.Name);
			}
		}
	}
}