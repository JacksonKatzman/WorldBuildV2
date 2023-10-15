using Game.Generators.Items;
using Game.Simulation;
using Game.Utilities;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class City : InertIncidentContext, IFactionAffiliated, ILocationAffiliated, IInventoryAffiliated
	{
		public override Type ContextType => typeof(City);
		public Location CurrentLocation { get; set; }
		public Faction AffiliatedFaction { get; set; }
		virtual public int Population
		{
			get
			{
				return (int)populationFloat;
			}
			set
			{
				populationFloat = (float)value;
			}
		}
		public int Wealth { get; set; }
		public int NumItems => CurrentInventory.Items.Count;
		public bool IsOnBorder => SimulationUtilities.FindBorderWithinFaction(AffiliatedFaction).Contains(CurrentLocation.TileIndex);
		public List<Resource> Resources { get; set; }
		public List<Landmark> Landmarks { get; set; }
		public List<Character> Characters { get; set; }

		public Inventory CurrentInventory { get; set; }
		private float populationFloat;

		public City() { }

		public City(Faction faction, Location location, int population, int wealth)
		{
			AffiliatedFaction = faction;
			CurrentLocation = location;
			Population = population;
			Wealth = wealth;
			Resources = new List<Resource>();
			Landmarks = new List<Landmark>();
			Characters = new List<Character>();
			CurrentInventory = new Inventory();

			if (ContextDictionaryProvider.CurrentContexts.GetContextByID(location.ID) == null)
			{
				EventManager.Instance.Dispatch(new AddContextEvent(location));
			}
		}

		public int GenerateWealth()
		{
			return 1;
		}

		public void UpdatePopulation()
		{
			populationFloat *= 1.011f;
		}

		public void GenerateMinorCharacters(int amount)
		{
			for(var i = 0; i < amount; i++)
			{
				var character = new Character(AffiliatedFaction);
				EventManager.Instance.Dispatch(new AddContextImmediateEvent(character));
			}
		}

		public override void LoadContextProperties()
		{
			CurrentLocation = SaveUtilities.ConvertIDToContext<Location>(contextIDLoadBuffers["CurrentLocation"][0]);
			AffiliatedFaction = SaveUtilities.ConvertIDToContext<Faction>(contextIDLoadBuffers["AffiliatedFaction"][0]);
			Resources = SaveUtilities.ConvertIDsToContexts<Resource>(contextIDLoadBuffers["Resources"]);
			Landmarks = SaveUtilities.ConvertIDsToContexts<Landmark>(contextIDLoadBuffers["Landmarks"]);
			Characters = SaveUtilities.ConvertIDsToContexts<Character>(contextIDLoadBuffers["Characters"]);
			CurrentInventory.LoadContextProperties();

			contextIDLoadBuffers.Clear();
		}
	}
}