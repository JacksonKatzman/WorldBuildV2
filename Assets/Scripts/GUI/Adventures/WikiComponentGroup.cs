using Game.Enums;
using System.Collections.Generic;

namespace Game.GUI.Adventures
{
    public class WikiComponentGroup : WikiComponent<string>
    {
        public List<WikiComponent<string>> components;
        public override void Clear()
        {
            foreach(var component in components)
            {
                component.Clear();
            }
        }

        protected override void Fill(string value)
        {
            foreach (var component in components)
            {
                component.Fill(value);
            }
        }

        public override void UpdateByFamiliarity(ContextFamiliarity familiarity)
        {
            ToggleCanvasGroup(familiarity >= FamiliarityRequirement);
        }

        private void ToggleCanvasGroup(bool active)
        {
            gameObject.SetActive(active);
        }
    }
}
