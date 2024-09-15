using Game.Enums;
using UnityEngine;

namespace Game.GUI.Adventures
{
    public class FamiliarityRequirementUIToggle : MonoBehaviour
    {
        [SerializeField]
        private ContextFamiliarity familiarityRequirement;

        private bool empty;

        public void Toggle(ContextFamiliarity familiarity)
        {
            if (!empty)
            {
                gameObject.SetActive(familiarity >= familiarityRequirement);
            }
        }

        public void SetEmpty(bool empty)
        {
            gameObject.SetActive(!empty);
            this.empty = empty;
        }
    }
}
