using Game.Simulation;

namespace Game.Incidents
{
	public class DemonCult : SpecialFaction
	{
		public override string Name => $"Cult of {Creator.CharacterName.firstName}";
        public override bool CanExpandTerritory => false;
		public override bool CanTakeMilitaryAction => false;
		public override string Description => $"Cult worshiping {Creator.Context.Link()}";
		protected override void OnRemoveContextEvent(RemoveContextEvent gameEvent)
		{
			base.OnRemoveContextEvent(gameEvent);
			if(Creator == gameEvent.context)
			{
				Die();
			}
		}
	}
}