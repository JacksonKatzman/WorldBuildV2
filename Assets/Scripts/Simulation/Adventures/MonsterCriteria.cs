using Game.Creatures;
using Game.Enums;
using Game.Incidents;
using Game.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Simulation
{
	[Serializable, HideReferenceObjectPicker]
	public class MonsterCriteria : AdventureContextCriteria<Monster>
	{
		public bool isLegendary;
		public bool isLandDwelling;

		[ValueDropdown("GetCreatureSizes", IsUniqueList = true, DropdownTitle = "Allowed Sizes")]
		public List<CreatureSize> allowedSizes;
		[ValueDropdown("GetCreatureTypes", IsUniqueList = true, DropdownTitle = "Allowed Types")]
		public List<CreatureType> allowedTypes;
		[ValueDropdown("GetCreatureAlignments", IsUniqueList = true, DropdownTitle = "Allowed Alignments")]
		public List<CreatureAlignment> allowedAlignments;

		public MonsterCriteria() : base()
		{
			allowedSizes = new List<CreatureSize>();
			allowedTypes = new List<CreatureType>();
			allowedAlignments = new List<CreatureAlignment>();
		}

		public MonsterData GetMonsterData()
		{
			var candidates = AdventureService.Instance.monsterData
				.Where(x => x.legendary == isLegendary)
				.Where(x => x.landDwelling == isLandDwelling)
				.Where(x => allowedSizes.Contains(x.size))
				.Where(x => allowedTypes.Contains(x.type))
				.Where(x => allowedAlignments.Contains(x.alignment))
				.ToList();

			return SimRandom.RandomEntryFromList(candidates);
		}

		private IEnumerable<CreatureSize> GetCreatureSizes()
		{
			return Enum.GetValues(typeof(CreatureSize)).Cast<CreatureSize>();
		}

		private IEnumerable<CreatureType> GetCreatureTypes()
		{
			return Enum.GetValues(typeof(CreatureType)).Cast<CreatureType>();
		}

		private IEnumerable<CreatureAlignment> GetCreatureAlignments()
		{
			return Enum.GetValues(typeof(CreatureAlignment)).Cast<CreatureAlignment>();
		}

		public override void RetrieveContext()
		{
			Context = new Monster();
			((Monster)Context).monsterData = GetMonsterData();
		}
	}
}
