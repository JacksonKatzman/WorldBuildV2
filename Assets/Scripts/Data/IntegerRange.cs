using Sirenix.OdinInspector;
using System;

namespace Game.Incidents
{
	[HideReferenceObjectPicker, InlineProperty]
	public class IntegerRange : ValueRange<int>
	{
		[HorizontalGroup("A", LabelWidth = 52, Width = 40)]
		public bool randomRange = true;

		[ShowIf("@this.randomRange"), HorizontalGroup("A", Width = 30)]
		public int min = 0, max = 20;
		[HideIf("@this.randomRange"), HorizontalGroup("A", Width = 30)]
		public int constant;

		public static implicit operator int(IntegerRange i) => i.Value;
		override public int Value
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

	public class FloatRange : ValueRange<float>
	{
		public override float Value => 0.0f;
	}

	public class BoolRange : ValueRange<bool>
	{
		public override bool Value => false;
	}

	public interface IValueRange 
	{
		Type GetValueType { get; }
	}

	public class ValueRange<T> : IValueRange
	{
		virtual public T Value => default(T);
		public Type GetValueType => typeof(T);
	}

	public static class ValueRangeFactory
	{
		public static IValueRange CreateValueRange<T>()
		{
			var type = typeof(T);
			if(type == typeof(int))
			{
				return new IntegerRange();
			}
			else if(type == typeof(float))
			{
				return new FloatRange();
			}
			else if(type == typeof(bool))
			{
				return new BoolRange();
			}
			else
			{
				return new ValueRange<T>();
			}
		}

		public static T FetchValue<T>(IValueRange range)
		{
			return ((ValueRange<T>)range).Value;
		}
	}
}