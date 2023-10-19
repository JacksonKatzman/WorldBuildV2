using System.Collections.Generic;
using Game.Data;
using Game.Generators.Items;
using Game.Incidents;
using Game.Simulation;
using NUnit.Framework;
using UnityEditor;

public class StandardCriteriaEvaluatorTests : Editor
{
    [SetUp]
    public void SetUp()
    {

        var current = new IncidentContextDictionary();
        var all = new IncidentContextDictionary();
        ContextDictionaryProvider.SetContextsProviders(() => current, () => all);
        ContextDictionaryProvider.SetNextID(1);
    }

    [TearDown]
    public void TearDown()
    {
        ContextDictionaryProvider.SetContextsProviders(() => null, () => null);
        ContextDictionaryProvider.SetNextID(1);
    }

    [Test]
    public void ListCountEvaluator()
    {
        var propertyName = "FactionsAtWarWith";
        var faction = new Faction() { ID = 1, FactionsAtWarWith = new List<IIncidentContext>() { new Faction() } };
        EventManager.Instance.Dispatch(new AddContextImmediateEvent(faction));

        var evaluator = new ListEvaluator(propertyName, typeof(Faction));
        evaluator.Comparator = "==";
        evaluator.expressions = new List<Expression<int>>();
        evaluator.expressions.Add(new Expression<int>() { constValue = 1 });

        var evaluation = evaluator.Evaluate(faction, propertyName);

        Assert.That(evaluation == true);

        faction.FactionsAtWarWith.Add(new Faction());
        evaluation = evaluator.Evaluate(faction, propertyName);

        Assert.That(evaluation == false);
    }

    [Test]
    public void IntegerEvaluator()
    {
        var propertyName = "GoodEvilAlignmentAxis";
        var faction = new Faction() { ID = 1, GoodEvilAlignmentAxis = 10 };
        EventManager.Instance.Dispatch(new AddContextImmediateEvent(faction));

        var evaluator = new IntegerEvaluator(propertyName, typeof(Faction));
        evaluator.Comparator = "==";
        evaluator.expressions = new List<Expression<int>>();
        evaluator.expressions.Add(new Expression<int>() { constValue = 10 });

        var evaluation = evaluator.Evaluate(faction, propertyName);

        Assert.That(evaluation == true);

        faction.GoodEvilAlignmentAxis += 1;
        evaluation = evaluator.Evaluate(faction, propertyName);

        Assert.That(evaluation == false);
    }

    [Test]
    public void FloatEvaluator()
    {
        var propertyName = "ValueMultiplier";
        var trinket = new Trinket() { ValueMultiplier = 1.0f };
        EventManager.Instance.Dispatch(new AddContextImmediateEvent(trinket));

        var evaluator = new FloatEvaluator(propertyName, typeof(Trinket));
        evaluator.Comparator = "==";
        evaluator.expressions = new List<Expression<float>>();
        evaluator.expressions.Add(new Expression<float>() { constValue = 1.0f });

        var evaluation = evaluator.Evaluate(trinket, propertyName);

        Assert.That(evaluation == true);

        trinket.ValueMultiplier += 0.5f;
        evaluation = evaluator.Evaluate(trinket, propertyName);

        Assert.That(evaluation == false);
    }

    [Test]
    public void BoolEvaluator()
    {
        var propertyName = "AtWar";
        var faction = new Faction() { ID = 1, FactionsAtWarWith = new List<IIncidentContext>() };
        EventManager.Instance.Dispatch(new AddContextImmediateEvent(faction));

        var evaluator = new BoolEvaluator(propertyName, typeof(Faction));
        evaluator.Comparator = "==";
        evaluator.expressions = new List<Expression<bool>>();
        evaluator.expressions.Add(new Expression<bool>() { constValue = true });

        var evaluation = evaluator.Evaluate(faction, propertyName);

        Assert.That(evaluation == false);

        faction.FactionsAtWarWith.Add(new Faction());
        evaluation = evaluator.Evaluate(faction, propertyName);

        Assert.That(evaluation == true);
    }

    [Test]
    public void PrimitiveDictionaryEvaluator()
    {
        var propertyName = "FactionRelations";
        var faction = new Faction();
        var opposingFaction = new Faction();

        faction.FactionRelations = new Dictionary<IIncidentContext, int>();
        faction.FactionRelations.Add(opposingFaction, 100);

        var evaluator = new IntegerValueDictionaryEvaluator(propertyName, typeof(Faction));
        evaluator.expressions = new List<Expression<int>>();
        evaluator.expressions.Add(new Expression<int>() { constValue = 10 });
        evaluator.Comparator = ">";
        var evaluation = evaluator.Evaluate(faction, propertyName);

        Assert.That(evaluation == true);

        evaluator.Comparator = "<";
        evaluation = evaluator.Evaluate(faction, propertyName);

        Assert.That(evaluation == false);
    }
}
