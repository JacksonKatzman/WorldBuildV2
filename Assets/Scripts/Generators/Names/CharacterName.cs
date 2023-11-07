using Game.Incidents;
using System;
using System.Collections.Generic;

namespace Game.Generators.Names
{
	public class CharacterName
	{
		public string nameFormat;
		public string firstName;
		public List<string> middleNames;
		public string surname;
		public string previousTitle;

		public string FirstName => firstName;
		public string Surname => surname;
		public string FullName => GetFullName();

		public CharacterName() { }
		public CharacterName(string format, string first, string last, List<string> middle)
		{
			nameFormat = format;
			firstName = first;
			surname = last;
			middleNames = middle;
		}

		public string GetFullName()
		{
			var fullName = firstName;
			foreach(var middle in middleNames)
			{
				fullName += $" {middle}";
			}
			fullName += $" {surname}";
			return fullName.Trim();
		}

		public string GetTitledFullName(ICharacter person)
		{
			var result = String.Copy(FullName);
			if (person.OrganizationPosition != null)
			{
				result = string.Format(person.OrganizationPosition.GetTitle(person), result);
			}
			return result;
		}

		public string GetPreviousTitledFullName(ICharacter person)
		{
			var result = String.Copy(FullName);
			if (!string.IsNullOrEmpty(previousTitle))
			{
				result = string.Format(previousTitle, result);
			}
			return result;
		}
	}
}