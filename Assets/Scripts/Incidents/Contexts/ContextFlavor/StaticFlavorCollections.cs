using Game.Enums;
using System.Collections.Generic;

namespace Game.Incidents
{
	public static class StaticFlavorCollections
	{
		public static Dictionary<Gender, string> objectGenderPronouns = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "him"}, {Gender.FEMALE, "her"}, {Gender.ANY, "them"}
		};

		public static Dictionary<Gender, string> subjectGenderPronouns = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "he"}, {Gender.FEMALE, "she"}, {Gender.ANY, "they"}
		};

		public static Dictionary<Gender, string> possessiveGenderPronouns = new Dictionary<Gender, string>()
		{
			{Gender.MALE, "his"}, {Gender.FEMALE, "her"}, {Gender.ANY, "their"}
		};
	}
}