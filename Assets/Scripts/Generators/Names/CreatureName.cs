using Game.Incidents;
using System;
using System.Collections.Generic;

namespace Game.Generators.Names
{
	public class CreatureName
	{
		public string nameFormat;
		public List<string> firstNames;
		public List<string> surnames;
		public string fullName;

		public string FirstName => firstNames[0];
		public string Surname => surnames[surnames.Count - 1];

		public CreatureName() { }
		public CreatureName(string format)
		{
			nameFormat = format;
			firstNames = new List<string>();
			surnames = new List<string>();
		}

		public string GetTitledFullName(IPerson person)
		{
			var result = String.Copy(fullName);
			if (person.OfficialPosition != null)
			{
				result = string.Format(person.OfficialPosition.titlePair.GetTitle(person.Gender), result);
			}
			return result;
		}
	}
}