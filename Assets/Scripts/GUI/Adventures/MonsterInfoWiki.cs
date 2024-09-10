using Game.Data;
using Game.Enums;
using Game.GUI.Wiki;
using System.Collections.Generic;

namespace Game.GUI.Adventures
{
    public class MonsterInfoWiki : StatBlockWikiComponent<MonsterData>
    {
        protected override ContextFamiliarity GetContextFamiliarity()
        {
            return ContextFamiliarity.TOTAL;
        }

        protected override string GetDescription()
        {
            var isLegendary = value.legendary == true ? "Legendary" : string.Empty;
            return $"{isLegendary} {value.size.ToString()} {value.type.ToString()}, {value.alignment.ToString()}";
        }

        protected override List<string> GetDetails()
        {
            return new List<string>();
        }

        protected override string GetName()
        {
            return value.monsterName;
        }

        protected override MonsterData GetStatBlock()
        {
            return value;
        }

        protected override SerializableStatBlock GetStats()
        {
            return value.stats;
        }

        protected override List<TooltipBox> GetTraits()
        {
            return new List<TooltipBox>();
        }
    }
}
