namespace Game.Utilities
{
	public static class StringUtilities
	{
		public static string ReplaceFirstOccurence(string source, string find, string replace)
		{
			int place = source.IndexOf(find);
			string result = source.Remove(place, find.Length).Insert(place, replace);
			return result.Replace("\r", "");
		}

		public static string ReplaceLastOccurrence(string source, string find, string replace)
		{
			int place = source.LastIndexOf(find);
			string result = source.Remove(place, find.Length).Insert(place, replace);
			return result;
		}
	}
}