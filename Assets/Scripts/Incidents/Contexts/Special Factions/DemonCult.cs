using Game.Simulation;

namespace Game.Incidents
{
	public class DemonCult : SpecialFaction
	{
		public override bool CanExpandTerritory => false;
		public override bool CanTakeMilitaryAction => false;

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