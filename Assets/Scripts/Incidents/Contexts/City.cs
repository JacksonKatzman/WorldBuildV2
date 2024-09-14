using Game.Generators.Items;
using Game.Simulation;
using Game.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class City : InertIncidentContext, IFactionAffiliated, ILocationAffiliated, IInventoryAffiliated
	{
		public override Type ContextType => typeof(City);
		public Location CurrentLocation { get; set; }
		public Faction AffiliatedFaction { get; set; }
		public override string Name { get; set; }
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
		public List<Landmark> Landmarks => ContextDictionaryProvider.GetCurrentContexts<Landmark>().Where(x => x.CurrentLocation.Equals(CurrentLocation)).ToList();
		public List<Character> Characters { get; set; }
		public override string Description => $"{WealthDenominationString} {SizeDenominationString} in {AffiliatedFaction.Link()}";

		public Inventory CurrentInventory { get; set; }
		public string SizeDenominationString => GetSizeDenomination();
		public string WealthDenominationString => GetWealthDenomination();
		private float populationFloat;

		public City() { }

		public City(Faction faction, Location location, int population, int wealth, string name = null)
		{
			AffiliatedFaction = faction;
			CurrentLocation = location;
			Population = population;
			Wealth = wealth;
			Resources = new List<Resource>();
			Characters = new List<Character>();
			CurrentInventory = new Inventory();
			Name = name;

			if(string.IsNullOrEmpty(Name))
			{
				Name = AffiliatedFaction.namingTheme.GenerateCityName();
			}

			if (ContextDictionaryProvider.CurrentContexts.GetContextByID(location.ID) == null)
			{
				EventManager.Instance.Dispatch(new AddContextEvent(location, false));
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
				EventManager.Instance.Dispatch(new AddContextEvent(character, true));
			}
		}

		public override void LoadContextProperties()
		{
			CurrentLocation = SaveUtilities.ConvertIDToContext<Location>(contextIDLoadBuffers["CurrentLocation"][0]);
			AffiliatedFaction = SaveUtilities.ConvertIDToContext<Faction>(contextIDLoadBuffers["AffiliatedFaction"][0]);
			Resources = SaveUtilities.ConvertIDsToContexts<Resource>(contextIDLoadBuffers["Resources"]);
			Characters = SaveUtilities.ConvertIDsToContexts<Character>(contextIDLoadBuffers["Characters"]);
			CurrentInventory.LoadContextProperties();

			contextIDLoadBuffers.Clear();
		}

		protected virtual string GetSizeDenomination()
        {
			//Todo: Change this code to be less temporary
			if(Population > 5000)
            {
				return "City";
            }
			else if(Population > 1000)
            {
				return "Town";
            }
			else
            {
				return "Hamlet";
            }
        }

		protected virtual string GetWealthDenomination()
        {
			//Todo: Change this code to be less temporary
			if (Wealth > 5000)
			{
				return "Prosperous";
			}
			else if (Wealth > 1000)
			{
				return "Bustling";
			}
			else
			{
				return "Poor";
			}
		}
	}
}