using Sirenix.OdinInspector;

namespace Game.Incidents
{
	[HideReferenceObjectPicker, InlineProperty]
	public class IntegerRange
	{
		[HorizontalGroup("A", LabelWidth = 52, Width = 40)]
		public bool randomRange = true;

		[ShowIf("@this.randomRange"), HorizontalGroup("A", Width = 30)]
		public int min = 0, max = 20;
		[HideIf("@this.randomRange"), HorizontalGroup("A", Width = 30)]
		public int constant;

		public static implicit operator int(IntegerRange i) => i.Value;
		public int Value
		{
			get
			{
				return randomRange ? Calculate() : constant;
			}
		}
		private int Calculate()
		{
			return SimRandom.RandomRange(min, max);
		}
	}
}