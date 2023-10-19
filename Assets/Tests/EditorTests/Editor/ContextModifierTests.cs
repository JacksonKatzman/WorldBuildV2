using Game.Data;
using Game.Incidents;
using Game.Simulation;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEditor;

public class ContextModifierTests : Editor
{
    [SetUp]
    public void SetUp()
    {
        var current = new IncidentContextDictionary();
        var all = new IncidentContextDictionary();
        ContextDictionaryProvider.SetContextsProviders(() => current, () => all);
        ContextDictionaryProvider.SetNextID(1);
        ContextDictionaryProvider.CurrentExpressionValues = new Dictionary<string, ExpressionValue>();
    }

    [TearDown]
    public void TearDown()
    {
        ContextDictionaryProvider.SetContextsProviders(() => null, () => null);
        ContextDictionaryProvider.SetNextID(1);
        ContextDictionaryProvider.CurrentExpressionValues.Clear();
    }

    //Do tests for the ContextModifier AND the calculators
    [Test]
    public void ContextModifierCalculatorUpdatesContextProperty()
	{
		var contextModifierCalculator = new IntegerContextModifierCalculator("Wealth", typeof(Character));
        contextModifierCalculator.Operation = "+";
        contextModifierCalculator.expressions[0].ExpressionType = ExpressionType.Const;
        contextModifierCalculator.expressions[0].constValue = 10;

        var character = new Character() { Wealth = 0 };
        contextModifierCalculator.Calculate(character);

        Assert.That(character.Wealth == 10);

        ContextDictionaryProvider.CurrentExpressionValues.Clear();

        contextModifierCalculator.clamped = true;
        contextModifierCalculator.expressions[0].constValue = -20;
        contextModifierCalculator.Calculate(character);

        Assert.That(character.Wealth == 0);
    }
}
