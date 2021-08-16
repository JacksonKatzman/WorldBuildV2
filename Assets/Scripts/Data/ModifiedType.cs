using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModifiedType<T> : ITimeSensitive
{
	protected T original;
	protected List<TimedTypeModifier<T>> addMods;
	protected List<TimedTypeModifier<T>> multMods;

	public T Original => original;
	public T Modified => CalculateModifiedAmount();

	public void AdvanceTime()
	{
		for(int i = 0; i < addMods.Count; i++)
		{
			addMods[i].duration--;
		}

		addMods.RemoveAll(mod => mod.duration == 0);

		for (int i = 0; i < addMods.Count; i++)
		{
			multMods[i].duration--;
		}

		multMods.RemoveAll(mod => mod.duration == 0);
	}

	public void AddModifier(ModificationType type, T amount, int duration)
	{
		//mods.Add(new TimedTypeModifier<T>(type, amount, duration));
		if(type == ModificationType.ADDITIVE)
		{
			addMods.Add(new TimedTypeModifier<T>(type, amount, duration));
		}
		else
		{
			multMods.Add(new TimedTypeModifier<T>(type, amount, duration));
		}
	}

	protected virtual T CalculateModifiedAmount()
	{
		return original;
	}
}

public class ModifiedFloat : ModifiedType<float>
{
	public ModifiedFloat(float original)
	{
		this.original = original;
		addMods = new List<TimedTypeModifier<float>>();
		multMods = new List<TimedTypeModifier<float>>();
	}

	protected override float CalculateModifiedAmount()
	{
		var current = original;
		foreach (var mod in multMods)
		{
			current *= mod.modificationAmount;
		}

		foreach (var mod in addMods)
		{
			current += mod.modificationAmount;
		}

		return current;
	}
}

public class ModifiedInt : ModifiedType<int>
{
	public ModifiedInt(int original)
	{
		this.original = original;
		addMods = new List<TimedTypeModifier<int>>();
		multMods = new List<TimedTypeModifier<int>>();
	}

	protected override int CalculateModifiedAmount()
	{
		var current = original;
		foreach (var mod in multMods)
		{
			current *= mod.modificationAmount;
		}

		foreach (var mod in addMods)
		{
			current += mod.modificationAmount;
		}

		return current;
	}
}

public class TimedTypeModifier<T>
{
	public ModificationType modificationType;
	public T modificationAmount;
	public int duration;

	public TimedTypeModifier(ModificationType modificationType, T modificationAmount, int duration)
	{
		this.modificationType = modificationType;
		this.modificationAmount = modificationAmount;
		this.duration = duration;
	}
}

public enum ModificationType { ADDITIVE, MULTIPLICATIVE };