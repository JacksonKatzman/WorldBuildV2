using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
	public static class CharacterExtensions
	{
		public static List<Character> GetExtendedFamily(Character c)
		{
			GameProfiler.BeginProfiling("GetExtendedFamily", GameProfiler.ProfileFunctionType.DEPLOY);
			var hashSet = new HashSet<Character>();
			CompileFamilyTree(c, 0, ref hashSet);
			if(hashSet.Contains(c))
			{
				hashSet.Remove(c);
			}
			GameProfiler.EndProfiling("GetExtendedFamily");
			return hashSet.ToList();
		}
		//check siblings - if got it then good
		//check spouses - if got it then good

		//check parents - if got it then good
		//check children - if got it then good

		// 1 - Spouse
		// 2 - Sibling
		// 3 - Parent
		// 4 - Child

		public static GeneologicalData FindRelationship(Character primaryCharacter, Character secondaryCharacter)
		{
			GameProfiler.BeginProfiling("FindRelationship", GameProfiler.ProfileFunctionType.DEPLOY);
			var possiblePaths = new List<GeneologicalData>();
			SearchFamilyTree(primaryCharacter, secondaryCharacter, 0, 0, 0, 0, ref possiblePaths);
			GeneologicalData toReturn;

			if(possiblePaths.Count == 0)
			{
				//not related
				toReturn = null;
			}
			else
			{
				var shortestPath = possiblePaths[0];
				for(int i = 1; i < possiblePaths.Count; i++)
				{
					shortestPath = shortestPath.iterations > possiblePaths[i].iterations ? possiblePaths[i] : shortestPath;
				}

				toReturn = shortestPath;
			}
			GameProfiler.EndProfiling("FindRelationship");
			return toReturn;
		}

		private static void SearchFamilyTree(Character searching, Character searchingFor, int verticallyRemoved, int siblingRemoved, int spouseRemoved, int iterations, ref List<GeneologicalData> data)
		{
			if(searching.Spouses.Contains(searchingFor))
			{
				data.Add(new GeneologicalData(verticallyRemoved, spouseRemoved, siblingRemoved, 1, iterations));
			}
			if(searching.Siblings.Contains(searchingFor))
			{
				data.Add(new GeneologicalData(verticallyRemoved, spouseRemoved, siblingRemoved, 2, iterations));
			}
			if (searching.Parents.Contains(searchingFor))
			{
				data.Add(new GeneologicalData(verticallyRemoved, spouseRemoved, siblingRemoved, 3, iterations));
			}
			if (searching.Children.Contains(searchingFor))
			{
				data.Add(new GeneologicalData(verticallyRemoved, spouseRemoved, siblingRemoved, 4, iterations));
			}

			if(iterations < 5)
			{
				//go again
				foreach(var spouse in searching.Spouses)
				{
					SearchFamilyTree(spouse, searchingFor, verticallyRemoved, siblingRemoved, spouseRemoved + 1, iterations + 1, ref data);
				}
				foreach (var sibling in searching.Siblings)
				{
					SearchFamilyTree(sibling, searchingFor, verticallyRemoved, siblingRemoved + 1, spouseRemoved, iterations + 1, ref data);
				}
				foreach (var parent in searching.Parents)
				{
					SearchFamilyTree(parent, searchingFor, verticallyRemoved - 1, siblingRemoved, spouseRemoved, iterations + 1, ref data);
				}
				foreach (var child in searching.Children)
				{
					SearchFamilyTree(child, searchingFor, verticallyRemoved + 1, siblingRemoved, spouseRemoved, iterations + 1, ref data);
				}
			}
		}
		private static void CompileFamilyTree(Character searching, int iterations, ref HashSet<Character> data)
		{
			if (iterations < 3)
			{
				foreach (var spouse in searching.Spouses)
				{
					data.Add(spouse);
					CompileFamilyTree(spouse, iterations + 1, ref data);
				}
				foreach (var sibling in searching.Siblings)
				{
					data.Add(sibling);
					CompileFamilyTree(sibling, iterations + 1, ref data);
				}
				foreach (var parent in searching.Parents)
				{
					data.Add(parent);
					CompileFamilyTree(parent, iterations + 1, ref data);
				}
				foreach (var child in searching.Children)
				{
					data.Add(child);
					CompileFamilyTree(child, iterations + 1, ref data);
				}
			}
		}
	}
}