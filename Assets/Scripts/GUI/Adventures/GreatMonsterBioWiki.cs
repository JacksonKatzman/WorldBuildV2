using Game.Data;
using Game.Enums;
using Game.GUI.Wiki;
using Game.Incidents;
using Game.Simulation;
using System.Collections.Generic;

namespace Game.GUI.Adventures
{
    public class GreatMonsterBioWiki : StatBlockWikiComponent<GreatMonster>
    {
        protected override ContextFamiliarity GetContextFamiliarity()
        {
            return AdventureService.Instance.IsDungeonMasterView ? ContextFamiliarity.TOTAL : value.Familiarity;
        }

        protected override string GetDescription()
        {
            return $"{value.Age.ToString()}, {value.Gender.ToString()}, Legendary {value.dataBlock.size.ToString()} {value.dataBlock.type.ToString()}, {value.dataBlock.alignment.ToString()}";
        }

        protected override List<string> GetDetails()
        {
            return new List<string>();
        }

        protected override string GetName()
        {
            return value.Name;
        }

        protected override MonsterData GetStatBlock()
        {
            return value.dataBlock;
        }

        protected override SerializableStatBlock GetStats()
        {
            return value.dataBlock.stats;
        }

        protected override List<TooltipBox> GetTraits()
        {
            return new List<TooltipBox>();
        }
    }
}
