using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Game.Incidents
{
	//[CreateAssetMenu(fileName = nameof(EditableCoreIncident), menuName = "ScriptableObjects/Incidents/" + nameof(EditableCoreIncident), order = 1)]
	public class EditableCoreIncident : SerializedScriptableObject
    {
        public string incidentName;
        public List<IIncidentTag> tags;
        public int weight;

        public List<IncidentModifier> required;
        public List<IncidentModifier> optional;
    }
}