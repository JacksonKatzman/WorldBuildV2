using Game.Enums;
using Game.Incidents;
using Game.Simulation;

namespace Game.GUI.Adventures
{
    public abstract class ContextWikiComponent<T> : WikiComponent<T> where T : IncidentContext
    {
        protected override void Fill(T value)
        {
            UpdateByFamiliarity(AdventureService.Instance.IsDungeonMasterView ? ContextFamiliarity.TOTAL : value.Familiarity);
        }

        protected override void Awake()
        {
            base.Awake();
            EventManager.Instance.AddEventHandler<IsDungeonMasterViewChangedEvent>(OnDungeonMasterViewChanges);
            LoadComponentList();
        }

        private void OnDungeonMasterViewChanges(IsDungeonMasterViewChangedEvent gameEvent)
        {
            UpdateByFamiliarity(gameEvent.isDungeonMasterView ? ContextFamiliarity.TOTAL : Value.Familiarity);
        }
    }
}
