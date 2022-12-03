using System;

namespace Game.Incidents
{
	public interface IContextModifierCalculator
    {
        Type PrimitiveType { get; }
        int ID { get; set; }
        string NameID { get; }
        void Calculate(IIncidentContext context);
    }
}