using Game.Data;
using Game.Enums;
using Game.GUI.Popups;
using Game.Incidents;
using Game.Utilities;
using Newtonsoft.Json;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Game.Simulation
{
	[Serializable, HideReferenceObjectPicker]
	public class MonsterRetriever : AdventureContextRetriever<Monster>
	{
		public bool findBySearch;

		[ShowIf("@this.findBySearch == false")]
		public ScriptableObjectRetriever<MonsterData> monsterData;
		[ShowIf("@this.findBySearch == true")]
		public bool isLegendary;
		[ShowIf("@this.findBySearch == true")]
		public bool isLandDwelling = true;

		[ValueDropdown("GetCreatureSizes", IsUniqueList = true, DropdownTitle = "Allowed Sizes"), ShowIf("@this.findBySearch == true")]
		public List<CreatureSize> allowedSizes;
		[ValueDropdown("GetCreatureTypes", IsUniqueList = true, DropdownTitle = "Allowed Types"), ShowIf("@this.findBySearch == true")]
		public List<CreatureType> allowedTypes;
		[ValueDropdown("GetCreatureAlignments", IsUniqueList = true, DropdownTitle = "Allowed Alignments"), ShowIf("@this.findBySearch == true")]
		public List<CreatureAlignment> allowedAlignments;


		[NonSerialized, JsonIgnore]
		private static readonly Dictionary<string, Func<Monster, int, string>> replacements = new Dictionary<string, Func<Monster, int, string>>
		{
			{"GROUPING", (monster, criteriaID) => monster.monsterData.groupingName },
			{"SOUND", (monster, criteriaID) => SimRandom.RandomEntryFromList(monster.monsterData.sounds) }
		};

		public MonsterRetriever() : base()
		{
			allowedSizes = new List<CreatureSize>();
			allowedTypes = new List<CreatureType>();
			allowedAlignments = new List<CreatureAlignment>();
		}

		public override Monster RetrieveContext()
		{
			var monster = new Monster();
			monster.monsterData = findBySearch? GetMonsterData() : monsterData?.RetrieveObject();
			return monster;
		}

		public MonsterData RetrieveMonsterData()
		{
			return findBySearch ? GetMonsterData() : monsterData.RetrieveObject();
		}

		public override void SpawnPopup()
		{
			var config = new MonsterInfoCardPopupConfig
			{
				MonsterData = TypedContext.monsterData
			};

			PopupService.Instance.ShowPopup(config);
		}

		override public void ReplaceTextPlaceholders(ref string text)
		{
			if (string.IsNullOrEmpty(text))
			{
				return;
			}

			HandleTextReplacements(ref text, replacements);
			base.ReplaceTextPlaceholders(ref text);
		}

		private MonsterData GetMonsterData()
		{
			/*
			var candidates = AdventureService.Instance.monsterData
				.Where(x => x.legendary == isLegendary)
				.Where(x => x.landDwelling == isLandDwelling)
				.Where(x => allowedSizes.Count > 0 ? allowedSizes.Contains(x.size) : true)
				.Where(x => allowedTypes.Count > 0 ? allowedTypes.Contains(x.type) : true)
				.Where(x => allowedAlignments.Count > 0 ? allowedAlignments.Contains(x.alignment) : true)
				.ToList();
			*/
			var candidates = AdventureService.Instance.monsterData
				.Where(x => x.legendary == isLegendary).ToList();
			candidates = candidates.Where(x => x.landDwelling == isLandDwelling).ToList();
			candidates = candidates.Where(x => allowedSizes.Count > 0 ? allowedSizes.Contains(x.size) : true).ToList();
			candidates = candidates.Where(x => allowedTypes.Count > 0 ? allowedTypes.Contains(x.type) : true).ToList();
			candidates = candidates.Where(x => allowedAlignments.Count > 0 ? allowedAlignments.Contains(x.alignment) : true).ToList();

			return SimRandom.RandomEntryFromList(candidates);
		}

		private string GetName()
		{
			return TypedContext.monsterData.name;
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
