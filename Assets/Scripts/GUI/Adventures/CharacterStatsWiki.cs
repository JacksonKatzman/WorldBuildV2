using Game.Incidents;
using Game.Simulation;
using TMPro;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class CharacterStatsWiki : ContextWikiComponent<Character>
    {
        [SerializeField]
        private TMP_Text characterName;
        [SerializeField]
        private StatContainerUI strengthStatContainer;
        [SerializeField]
        private StatContainerUI dexterityStatContainer;
        [SerializeField]
        private StatContainerUI constitutionStatContainer;
        [SerializeField]
        private StatContainerUI intelligenceStatContainer;
        [SerializeField]
        private StatContainerUI wisdomStatContainer;
        [SerializeField]
        private StatContainerUI charismaStatContainer;
        public override void Clear()
        {
            foreach (var component in componentList)
            {
                component.Clear();
            }
        }

        protected override void Fill(Character value)
        {
            characterName.text = value.CharacterName.GetFullName();

            strengthStatContainer.Fill(value.Strength);
            dexterityStatContainer.Fill(value.Dexterity);
            constitutionStatContainer.Fill(value.Constitution);
            intelligenceStatContainer.Fill(value.Intelligence);
            wisdomStatContainer.Fill(value.Wisdom);
            charismaStatContainer.Fill(value.Charisma);
        }
    }
}
