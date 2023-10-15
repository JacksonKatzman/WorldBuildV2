using System.Collections.Generic;
using Game.Enums;
using Game.Generators.Items;
using Game.Incidents;
using Game.Simulation;
using NUnit.Framework;
using UnityEditor;

public class ActionFieldCriteriaEvaluatorTests : Editor
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
    public void ActionFieldIntDictionaryEvaluator()
    {
        var propertyName = "FactionRelations";
        var faction = new Faction();
        var opposingFaction = new Faction();

        faction.FactionRelations = new Dictionary<IIncidentContext, int>();
        faction.FactionRelations.Add(opposingFaction, 100);

        var evaluator = new ActionFieldIntDictionaryEvaluator(propertyName, typeof(Faction));
        evaluator.value = 50;
        evaluator.Comparator = ">";
        var evaluation = evaluator.Evaluate(opposingFaction, propertyName, faction);

        Assert.That(evaluation == true);

        evaluator.Comparator = "<";
        evaluation = evaluator.Evaluate(opposingFaction, propertyName, faction);

        Assert.That(evaluation == false);

        var otherFaction = new Faction();
        evaluation = evaluator.Evaluate(otherFaction, propertyName, faction);

        Assert.That(evaluation == false);
    }

    [Test]
    public void ActionFieldListContainsEvaluator()
	{
        var propertyName = "FactionsAtWarWith";
        var faction = new Faction();
        var opposingFaction = new Faction();

        faction.FactionsAtWarWith = new List<IIncidentContext>();
        faction.FactionsAtWarWith.Add(opposingFaction);

        var evaluator = new ActionFieldListContainsEvaluator(propertyName, typeof(Faction));
        evaluator.Comparator = "==";
        var evaluation = evaluator.Evaluate(opposingFaction, propertyName, faction);

        Assert.That(evaluation == true);

        evaluator.Comparator = "!=";
        evaluation = evaluator.Evaluate(opposingFaction, propertyName, faction);

        Assert.That(evaluation == false);

        var otherFaction = new Faction();
        evaluation = evaluator.Evaluate(otherFaction, propertyName, faction);

        Assert.That(evaluation == true);

        evaluator.Comparator = "==";
        evaluation = evaluator.Evaluate(otherFaction, propertyName, faction);

        Assert.That(evaluation == false);
    }

    [Test]
    public void FactionContextMatchesEvaluator()
	{
        var propertyName = "AffiliatedFaction";
        var faction = new Faction() { ID = 1 };
        EventManager.Instance.Dispatch(new AddContextImmediateEvent(faction));

        var character = new Character() { AffiliatedFaction = faction };
        var city = new City() { AffiliatedFaction = faction };
        var monster = new GreatMonster() { AffiliatedFaction = new Faction() { ID = 5 } };
        var weapon = new Weapon();

        var evaluator = new FactionEvaluator(propertyName, typeof(Faction));
        evaluator.compareTo = new InterfacedIncidentActionFieldContainer<IFactionAffiliated>();
        var field = new ContextualIncidentActionField<Faction>();
        field.AllowSelf = true;
        evaluator.compareTo.actionField = field;
        evaluator.compareTo.actionField.CalculateField(faction);

        evaluator.Comparator = "==";
        var evaluation = evaluator.Evaluate(faction, propertyName, null);

        Assert.That(evaluation == true);

        evaluation = evaluator.Evaluate(character, propertyName, null);

        Assert.That(evaluation == true);

        evaluation = evaluator.Evaluate(city, propertyName, null);

        Assert.That(evaluation == true);

        evaluator.Comparator = "!=";
        evaluation = evaluator.Evaluate(city, propertyName, null);

        Assert.That(evaluation == false);

        evaluator.Comparator = "==";
        evaluation = evaluator.Evaluate(monster, propertyName, null);

        Assert.That(evaluation == false);

        evaluation = evaluator.Evaluate(weapon, propertyName, null);

        Assert.That(evaluation == false);
    }

    [Test]
    public void InventoryContextMatchesEvaluator()
    {
        var propertyName = "CurrentInventory";
        var inventory = new Inventory() { ID = 1 };
        EventManager.Instance.Dispatch(new AddContextImmediateEvent(inventory));

        var character = new Character() { CurrentInventory = inventory};
        var city = new City() { CurrentInventory = inventory };
        var monster = new GreatMonster() { CurrentInventory = new Inventory() { ID = 5 } };

        var evaluator = new InventoryEvaluator(propertyName, typeof(Inventory));
        evaluator.compareTo = new InterfacedIncidentActionFieldContainer<IInventoryAffiliated>();
        var field = new ContextualIncidentActionField<Inventory>();
        field.AllowSelf = true;
        evaluator.compareTo.actionField = field;

        evaluator.Comparator = "==";
        var evaluation = evaluator.Evaluate(inventory, propertyName, null);

        Assert.That(evaluation == true);

        evaluation = evaluator.Evaluate(character, propertyName, null);

        Assert.That(evaluation == true);

        evaluation = evaluator.Evaluate(city, propertyName, null);

        Assert.That(evaluation == true);

        evaluator.Comparator = "!=";
        evaluation = evaluator.Evaluate(city, propertyName, null);

        Assert.That(evaluation == false);

        evaluator.Comparator = "==";
        evaluation = evaluator.Evaluate(monster, propertyName, null);

        Assert.That(evaluation == false);
    }

    [Test]
    public void LocationContextMatchesEvaluator()
    {
        var propertyName = "CurrentLocation";
        var location = new Location() { TileIndex = 1 };
        EventManager.Instance.Dispatch(new AddContextImmediateEvent(location));

        var landmark = new Landmark() { CurrentLocation = location };
        var city = new City() { CurrentLocation = new Location { TileIndex = 9 } };

        var evaluator = new LocationEvaluator(propertyName, typeof(Inventory));
        evaluator.compareTo = new InterfacedIncidentActionFieldContainer<ILocationAffiliated>();
        var field = new ContextualIncidentActionField<Location>();
        field.AllowSelf = true;
        evaluator.compareTo.actionField = field;

        evaluator.Comparator = "==";
        var evaluation = evaluator.Evaluate(location, propertyName, null);

        Assert.That(evaluation == true);

        evaluation = evaluator.Evaluate(landmark, propertyName, null);

        Assert.That(evaluation == true);


        evaluator.Comparator = "!=";
        evaluation = evaluator.Evaluate(landmark, propertyName, null);

        Assert.That(evaluation == false);

        evaluator.Comparator = "==";
        evaluation = evaluator.Evaluate(city, propertyName, null);

        Assert.That(evaluation == false);
    }

    [Test]
    public void TypeEvaluator()
	{
        var propertyName = "FactionType";
        var faction = new Faction() { ID = 1 };
        EventManager.Instance.Dispatch(new AddContextImmediateEvent(faction));

        var evaluator = new TypeEvaluator(propertyName, typeof(Faction));
        evaluator.Comparator = "==";
        evaluator.comparedType = typeof(Faction);

        var evaluation = evaluator.Evaluate(faction, propertyName);

        Assert.That(evaluation == true);

        evaluator.comparedType = typeof(Character);
        evaluation = evaluator.Evaluate(faction, propertyName);

        Assert.That(evaluation == false);
    }

    [Test]
    public void EnumEvaluator()
	{
        var propertyName = "ItemGrade";
        var armor = new Armor() { ItemGrade = Game.Enums.ItemGrade.NORMAL };
        EventManager.Instance.Dispatch(new AddContextImmediateEvent(armor));

        var evaluator = new EnumEvaluator<ItemGrade>(propertyName, typeof(Armor));
        evaluator.allowedValues = new List<ItemGrade>() { ItemGrade.NORMAL, ItemGrade.AWFUL };
        var evaluation = evaluator.Evaluate(armor, propertyName);

        Assert.That(evaluation == true);

        evaluator.allowedValues.Remove(ItemGrade.NORMAL);
        evaluation = evaluator.Evaluate(armor, propertyName);

        Assert.That(evaluation == false);
    }
}
