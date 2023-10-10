using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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

		public static string CapitalizeAfter(this string s, IEnumerable<char> chars)
		{
			var charsHash = new HashSet<char>(chars);
			StringBuilder sb = new StringBuilder(s);
			for (int i = 0; i < sb.Length - 2; i++)
			{
				if (charsHash.Contains(sb[i]) && sb[i + 1] == ' ')
					sb[i + 2] = char.ToUpper(sb[i + 2]);
			}
			return sb.ToString();
		}

		public static string CapitalizeAfterStandard(this string s)
		{
			return s.CapitalizeAfter(new[] { '.', ':', '?', '!' });
		}

		public static string CapitalizeAfterLink(this string s)
		{
			var regex = @"\. " + "<link=\\\"" + @"(\d+)" + "\\\">[a-z]";
			return Regex.Replace(s, regex, delegate (Match match)
			{
				string v = match.ToString();
				return v.Substring(0, v.Length - 1) + char.ToUpper(v[v.Length - 1]);
			});
		}
	}
}