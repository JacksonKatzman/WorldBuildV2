using Game.Creatures;
using Game.Enums;
using Game.GUI.Popups;
using Game.Incidents;
using Game.Utilities;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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
		override public Dictionary<string, Func<Monster, string>> Replacements => replacements;

		private static Dictionary<string, Func<Monster, string>> replacements = new Dictionary<string, Func<Monster, string>>
		{
			{"{##}", (monster) => string.Format("<i><link=\"{0}\">{1}</link></i>", monster.ID, monster.monsterData.name.ToLower()) },
			{"-##-", (monster) => monster.monsterData.groupingName },
			{"<##>", (monster) => SimRandom.RandomEntryFromList(monster.monsterData.sounds) }
		};

		public MonsterCriteria() : base()
		{
			allowedSizes = new List<CreatureSize>();
			allowedTypes = new List<CreatureType>();
			allowedAlignments = new List<CreatureAlignment>();
		}

		public override void RetrieveContext()
		{
			Context = new Monster();
			((Monster)Context).monsterData = GetMonsterData();
		}

		public override void SpawnPopup()
		{
			var config = new MonsterInfoCardPopupConfig
			{
				MonsterData = TypedContext.monsterData
			};

			PopupService.Instance.ShowPopup(config);
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

		private string GetName()
		{
			return GetTypedContext().monsterData.name;
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
	}
}
