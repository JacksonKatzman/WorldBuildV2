namespace Game.Incidents
{
	public interface IContextModifierCalculator
    {
        void Calculate(IIncidentContext context);
    }
}