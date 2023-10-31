using Game.Enums;
using System;

namespace Game.Incidents
{
	public class GenderEvaluator : SpecializedEnumEvaluator<Gender, ISentient>
	{
		public GenderEvaluator() : base() { }
		public GenderEvaluator(string propertyName, Type contextType) : base(propertyName, contextType) { }

		protected override Gender GetEnumValue(IIncidentContext context)
		{
			if ((typeof(ISentient)).IsAssignableFrom(context.GetType()))
			{
				return ((ISentient)context).Gender;
			}
			else
			{
				return Gender.ANY;
			}
		}
	}
}