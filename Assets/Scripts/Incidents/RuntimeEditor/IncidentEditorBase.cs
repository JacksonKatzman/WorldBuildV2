using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
    public class IncidentEditorBase : IRuntimeEditorCompatible
    {
        [ValueDropdown("GetFilteredTypeList")]
        public Type incidentContextType;

        public NullEditorCompatibleComponent nECC;

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
    }
}