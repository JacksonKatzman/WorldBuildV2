using Game.Debug;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Incidents
{
    public class IncidentEditorBase : IRuntimeEditorCompatible
    {
        [ValueDropdown("GetFilteredTypeList"), OnValueChanged("SetContextType")]
        public Type incidentContextType;

        public IIncidentWeight weight;

        public string someInput;
        [TextArea(5,10)]
        public string someBiggerInput;
        public float someFloat;
        [Range(0.0f, 2.2f)]
        public float someRangedFloat;
        public int someInt;
        [Range(4,8)]
        public int someRangedInt;
        public bool someBool;

        private IEnumerable<Type> GetFilteredTypeList()
        {
            var q = typeof(IIncidentContext).Assembly.GetTypes()
                .Where(x => !x.IsAbstract)                                          // Excludes BaseClass
                .Where(x => !x.IsGenericTypeDefinition)                             // Excludes Generics
                .Where(x => typeof(IIncidentContext).IsAssignableFrom(x))           // Excludes classes not inheriting from IIncidentContext
                .Where(x => !typeof(InertIncidentContext).IsAssignableFrom(x))      // Excludes inert contexts
                .Where(x => x.BaseType != typeof(SpecialFaction));                  // Excludes special factions for now

            return q;
        }

        private void SetContextType()
        {
            //ContextType = incidentContextType;
            //criteria = new List<IIncidentCriteria>();
            //worldCriteria = new List<IIncidentCriteria>();
            //actionHandler = new IncidentActionHandlerContainer(ContextType);
            CreateIncidentWeight();
        }

        private void CreateIncidentWeight()
        {
            var dataType = new Type[] { incidentContextType };
            var genericBase = typeof(IncidentWeight<>);
            var combinedType = genericBase.MakeGenericType(dataType);
            //weight = (IIncidentWeight)Activator.CreateInstance(combinedType);
        }
    }
}