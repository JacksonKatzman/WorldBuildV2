using Game.Enums;
using Game.Factions;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using UnityEngine;

namespace Game.Incidents
{
	public interface SearchContext<T> : IModifierInfoContainer
	{
		public abstract List<T> EvaluateSearch(List<T> searchables);
	}

	public enum NumberComparisonOperator { MORE, LESS, SAME }

	public class AgeSearchContext : SearchContext<Person>, SearchContext<ILandmark>, SearchContext<OldFaction>
	{
		public System.Numerics.Vector2 ageRange;
		public List<Person> EvaluateSearch(List<Person> searchables)
		{
			return searchables.Where(x => x.Age >= ageRange.X && x.Age <= ageRange.Y).ToList();
		}

		public List<ILandmark> EvaluateSearch(List<ILandmark> searchables)
		{
			return searchables.Where(x => x.Age >= ageRange.X && x.Age <= ageRange.Y).ToList();
		}

		public List<OldFaction> EvaluateSearch(List<OldFaction> searchables)
		{
			return searchables.Where(x => x.Age >= ageRange.X && x.Age <= ageRange.Y).ToList();
		}
	}

	public class FactionSearchContext : SearchContext<Person>, SearchContext<ILandmark>, IFactionContainer
	{
		private List<OldFaction> factions = new List<OldFaction>();

		[HideInInspector]
		public List<OldFaction> Factions => factions;
		public List<Person> EvaluateSearch(List<Person> searchables)
		{
			return searchables.Where(x => x.faction == factions.FirstOrDefault()).ToList();
		}

		public List<ILandmark> EvaluateSearch(List<ILandmark> searchables)
		{
			return searchables.Where(x => x.Faction == factions.FirstOrDefault()).ToList();
		}
	}

	public class HaveNumChildrenSearchContext : SearchContext<Person>
	{
		int numChildren;
		NumberComparisonOperator comparisonOperator;
		public List<Person> EvaluateSearch(List<Person> searchables)
		{
			if(comparisonOperator == NumberComparisonOperator.MORE)
			{
				return searchables.Where(x => x.children.Count > numChildren).ToList();
			}
			else if (comparisonOperator == NumberComparisonOperator.LESS)
			{
				return searchables.Where(x => x.children.Count < numChildren).ToList();
			}
			else
			{
				return searchables.Where(x => x.children.Count == numChildren).ToList();
			}
		}
	}

	public class GenderSearchContext : SearchContext<Person>
	{
		public Gender gender;
		public List<Person> EvaluateSearch(List<Person> searchables)
		{
			return searchables.Where(x => x.gender == gender).ToList();
		}
	}

	public class LeadershipSearchContext : SearchContext<Person>
	{
		public List<Person> EvaluateSearch(List<Person> searchables)
		{
			return searchables.Where(x => x.governmentOffice != null).ToList();
		}
	}
	public class HasInfluenceSearchContext : SearchContext<Person>, SearchContext<OldFaction>
	{
		int numInfluence;
		NumberComparisonOperator comparisonOperator;
		public List<Person> EvaluateSearch(List<Person> searchables)
		{
			if (comparisonOperator == NumberComparisonOperator.MORE)
			{
				return searchables.Where(x => x.influence > numInfluence).ToList();
			}
			else if (comparisonOperator == NumberComparisonOperator.LESS)
			{
				return searchables.Where(x => x.influence < numInfluence).ToList();
			}
			else
			{
				return searchables.Where(x => x.influence == numInfluence).ToList();
			}
		}

		public List<OldFaction> EvaluateSearch(List<OldFaction> searchables)
		{
			if (comparisonOperator == NumberComparisonOperator.MORE)
			{
				return searchables.Where(x => x.influence > numInfluence).ToList();
			}
			else if (comparisonOperator == NumberComparisonOperator.LESS)
			{
				return searchables.Where(x => x.influence < numInfluence).ToList();
			}
			else
			{
				return searchables.Where(x => x.influence == numInfluence).ToList();
			}
		}
	}

	public class HasInventorySearchContext : SearchContext<Person>, SearchContext<ILandmark>
	{
		public List<Person> EvaluateSearch(List<Person> searchables)
		{
			return searchables.Where(x => x.Inventory.Count > 0).ToList();
		}

		public List<ILandmark> EvaluateSearch(List<ILandmark> searchables)
		{
			return searchables.Where(x => x.Inventory.Count > 0).ToList();
		}
	}

	public class PopulationSearchContext : SearchContext<City>, SearchContext<OldFaction>
	{
		int popRequirement;
		NumberComparisonOperator comparisonOperator;
		public List<City> EvaluateSearch(List<City> searchables)
		{
			if (comparisonOperator == NumberComparisonOperator.MORE)
			{
				return searchables.Where(x => x.population > popRequirement).ToList();
			}
			else if (comparisonOperator == NumberComparisonOperator.LESS)
			{
				return searchables.Where(x => x.population < popRequirement).ToList();
			}
			else
			{
				return searchables.Where(x => x.population == popRequirement).ToList();
			}
		}

		public List<OldFaction> EvaluateSearch(List<OldFaction> searchables)
		{
			if (comparisonOperator == NumberComparisonOperator.MORE)
			{
				return searchables.Where(x => x.population > popRequirement).ToList();
			}
			else if (comparisonOperator == NumberComparisonOperator.LESS)
			{
				return searchables.Where(x => x.population < popRequirement).ToList();
			}
			else
			{
				return searchables.Where(x => x.population == popRequirement).ToList();
			}
		}
	}

	public class HaveNumCitiesSearchContext : SearchContext<OldFaction>
	{
		int numCities;
		NumberComparisonOperator comparisonOperator;
		public List<OldFaction> EvaluateSearch(List<OldFaction> searchables)
		{
			if (comparisonOperator == NumberComparisonOperator.MORE)
			{
				return searchables.Where(x => x.cities.Count > numCities).ToList();
			}
			else if (comparisonOperator == NumberComparisonOperator.LESS)
			{
				return searchables.Where(x => x.cities.Count < numCities).ToList();
			}
			else
			{
				return searchables.Where(x => x.cities.Count == numCities).ToList();
			}
		}
	}

	public class HaveNumTilesSearchContext : SearchContext<OldFaction>
	{
		int numTiles;
		NumberComparisonOperator comparisonOperator;
		public List<OldFaction> EvaluateSearch(List<OldFaction> searchables)
		{
			if (comparisonOperator == NumberComparisonOperator.MORE)
			{
				return searchables.Where(x => x.territory.Count > numTiles).ToList();
			}
			else if (comparisonOperator == NumberComparisonOperator.LESS)
			{
				return searchables.Where(x => x.territory.Count < numTiles).ToList();
			}
			else
			{
				return searchables.Where(x => x.territory.Count == numTiles).ToList();
			}
		}
	}
}