using Sirenix.OdinInspector;

namespace Game.Incidents
{
	public class FactionBattleContext : DeployableContext
	{
		[HideReferenceObjectPicker]
		public DeployedContextActionField<Faction> attacker;
		[HideReferenceObjectPicker]
		public DeployedContextActionField<Faction> defender;
	}
}