using Game.Incidents;
using System;
using System.Collections.Generic;

namespace Game.Generators.Names
{
	public class CharacterName
	{
		public string nameFormat;
		public List<string> firstNames;
		public List<string> surnames;
		public string fullName;
		public string previousTitle;

		public string FirstName => firstNames[0];
		public string Surname => surnames[surnames.Count - 1];

		public CharacterName() { }
		public CharacterName(string format)
		{
			nameFormat = format;
			firstNames = new List<string>();
			surnames = new List<string>();
		}

		public string GetTitledFullName(ICharacter person)
		{
			var result = String.Copy(fullName);
			if (person.OrganizationPosition != null)
			{
				result = string.Format(person.OrganizationPosition.GetTitle(person), result);
			}
			return result;
		}

		public string GetPreviousTitledFullName(ICharacter person)
		{
			var result = String.Copy(fullName);
			if (!string.IsNullOrEmpty(previousTitle))
			{
				result = string.Format(previousTitle, result);
			}
			return result;
		}
	}
}