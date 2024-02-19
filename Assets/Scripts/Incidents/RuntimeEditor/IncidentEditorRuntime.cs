using Game.Debug;
using Sirenix.OdinInspector;
using System.Reflection;
using UnityEngine;

namespace Game.Incidents
{
    public interface IRuntimeEditorComponent { }
    public interface IRuntimeEditorCompatible { }
    public class IncidentEditorRuntime : SerializedMonoBehaviour, IRuntimeEditorCompatible
    {
        [SerializeField]
        private Transform editorRoot;

        public IncidentEditorBase incidentEditorBase;

        [Button("Create New Incident")]
        public void CreateNewIncident()
        {
            if(editorRoot.childCount > 0)
            {
                for(int i = 0; i < editorRoot.childCount; i++)
                {
                    Destroy(editorRoot.GetChild(i).gameObject);
                }
            }

            var fieldInfo = GetType().GetField("incidentEditorBase");
            var block = Instantiate(RuntimeEditorPrefabs.Instance.blockPrefab, editorRoot);
            block.Initialize(fieldInfo, this);
        }
    }
}