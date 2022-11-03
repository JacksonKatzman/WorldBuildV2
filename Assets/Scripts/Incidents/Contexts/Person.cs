using Game.Enums;
using Game.Generators.Items;
using Game.Incidents;
using System;
using System.Collections.Generic;

namespace Game.Incidents
{
	public class Person : IIncidentContext
	{
		public Type ContextType => typeof(Person);

		public int NumIncidents { get; set; }

		public int ParentID => -1;
		public int Age { get; set; }
		public Gender Gender { get; set; }
		public Race Race { get; set; }
		public int PoliticalPriority { get; set; }
		public int EconomicPriority { get; set; }
		public int ReligiousPriority { get; set; }
		public int MilitaryPriority { get; set; }
		public int Influence { get; set; }
		public int Wealth { get; set; }
		public int Strength { get; set; }
		public int Dexterity { get; set; }
		public int Constitution { get; set; }
		public int Intelligence { get; set; }
		public int Wisdom { get; set; }
		public int Charisma { get; set; }
		public List<Item> Inventory { get; set; }
		public List<Person> Parents { get; set; }
		public List<Person> Spouses { get; set; }
		public List<Person> Children { get; set; }
		public void DeployContext()
		{
			
		}

		public void UpdateContext()
		{
			
		}
	}

	public class Race { }
}