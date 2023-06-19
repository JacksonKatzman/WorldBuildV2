using System.Collections.Generic;
using Game.Enums;
using Game.Generators.Items;
using Game.Incidents;
using Game.Simulation;
using NUnit.Framework;
using UnityEditor;

public class ExpressionTests : Editor
{
	[Test]
	public void GetExpressionValueByMethod()
	{
		var expression = new Expression<int>();
		expression.ExpressionType = ExpressionType.Method;
		expression.chosenMethod = "FindSomeInteger";

		var value = expression.GetValue(null, null);
		var expected = 0;

		Assert.That(value == expected);
	}

	[Test]
	public void GetExpressionValueByProperty()
	{
		var faction = new Faction() { ID = 1 };
		var expression = new Expression<int>();
		expression.ExpressionType = ExpressionType.Property;
		expression.chosenProperty = "ID";

		var value = expression.GetValue(faction, null);
		var expected = 1;

		Assert.That(value == expected);
	}

	[Test]
	public void GetExpressionValueWithinRange()
	{
		var expression = new Expression<int>();
		expression.ExpressionType = ExpressionType.Range;
		expression.range = new IntegerRange() { randomRange = false, constant = 1 };

		var value = expression.GetValue(null, null);
		var expected = 1;

		Assert.That(value == expected);
	}

	[Test]
	public void GetExpressionValueByPreviousID()
	{
		var expression = new Expression<int>();
		expression.ExpressionType = ExpressionType.From_Previous;
		expression.previousCalculatedID = "1"; 

		ContextDictionaryProvider.CurrentExpressionValues = new Dictionary<string, ExpressionValue>();
		ContextDictionaryProvider.CurrentExpressionValues.Add("1", new ExpressionValue(1));

		var value = expression.GetValue(null, null);
		var expected = 1;

		Assert.That(value == expected);
	}

	[Test]
	public void CombineExpressions()
	{
		var expression_1 = new Expression<int>() { constValue = 2 };
		expression_1.ExpressionType = ExpressionType.Const;
		expression_1.nextOperator = "+";

		var expression_2 = new Expression<int>() { constValue = 2 };
		expression_2.ExpressionType = ExpressionType.Const;
		expression_2.nextOperator = "*";

		var expression_3 = new Expression<int>() { constValue = 2 };
		expression_3.ExpressionType = ExpressionType.Const;
		expression_3.nextOperator = "-";

		var expression_4 = new Expression<int>() { constValue = 2 };
		expression_4.ExpressionType = ExpressionType.Const;
		expression_4.nextOperator = "/";

		var expression_5 = new Expression<int>() { constValue = 2 };
		expression_5.ExpressionType = ExpressionType.Const;

		var expressions = new List<Expression<int>>() { expression_1, expression_2, expression_3, expression_4, expression_5 };
		var value = Expression<int>.CombineExpressions(null, expressions, ExpressionHelpers.IntegerOperators);
		var expected = 3;

		Assert.That(value == expected);

		var expression_6 = new Expression<int>();
		expression_6.ExpressionType = ExpressionType.Subexpression;
		expression_6.subexpressions = expressions;

		var withSubexpressions = new List<Expression<int>>() { expression_1, expression_6 };
		value = Expression<int>.CombineExpressions(null, withSubexpressions, ExpressionHelpers.IntegerOperators);
		expected = 5;

		Assert.That(value == expected);
	}

	[Test]
	public void IntegerOperators()
	{
		Assert.That(ExpressionHelpers.IntegerOperators["+"].Invoke(2, 2) == 4);
		Assert.That(ExpressionHelpers.IntegerOperators["-"].Invoke(2, 2) == 0);
		Assert.That(ExpressionHelpers.IntegerOperators["*"].Invoke(2, 2) == 4);
		Assert.That(ExpressionHelpers.IntegerOperators["/"].Invoke(2, 2) == 1);
		Assert.That(ExpressionHelpers.IntegerOperators["^"].Invoke(2, 2) == 4);
		Assert.That(ExpressionHelpers.IntegerOperators["="].Invoke(2, 2) == 2);
	}

	[Test]
	public void FloatOperators()
	{
		Assert.That(ExpressionHelpers.FloatOperators["+"].Invoke(2, 2) == 4);
		Assert.That(ExpressionHelpers.FloatOperators["-"].Invoke(2, 2) == 0);
		Assert.That(ExpressionHelpers.FloatOperators["*"].Invoke(2, 2) == 4);
		Assert.That(ExpressionHelpers.FloatOperators["/"].Invoke(2, 2) == 1);
		Assert.That(ExpressionHelpers.FloatOperators["^"].Invoke(2, 2) == 4);
	}

	[Test]
	public void BoolOperators()
	{
		Assert.That(ExpressionHelpers.BoolOperators["&&"].Invoke(true, true) == true);
		Assert.That(ExpressionHelpers.BoolOperators["&&"].Invoke(true, false) == false);
		Assert.That(ExpressionHelpers.BoolOperators["&&"].Invoke(false, false) == false);
		Assert.That(ExpressionHelpers.BoolOperators["||"].Invoke(true, true) == true);
		Assert.That(ExpressionHelpers.BoolOperators["||"].Invoke(true, false) == true);
		Assert.That(ExpressionHelpers.BoolOperators["||"].Invoke(false, false) == false);
	}

	[Test]
	public void IntegerComparators()
	{
		Assert.That(ExpressionHelpers.IntegerComparators[">"].Invoke(2, 1) == true);
		Assert.That(ExpressionHelpers.IntegerComparators[">"].Invoke(1, 1) == false);
		Assert.That(ExpressionHelpers.IntegerComparators[">="].Invoke(2, 1) == true);
		Assert.That(ExpressionHelpers.IntegerComparators[">="].Invoke(1, 1) == true);
		Assert.That(ExpressionHelpers.IntegerComparators["<"].Invoke(1, 2) == true);
		Assert.That(ExpressionHelpers.IntegerComparators["<"].Invoke(1, 1) == false);
		Assert.That(ExpressionHelpers.IntegerComparators["<="].Invoke(1, 2) == true);
		Assert.That(ExpressionHelpers.IntegerComparators["<="].Invoke(1, 1) == true);
		Assert.That(ExpressionHelpers.IntegerComparators["=="].Invoke(2, 1) == false);
		Assert.That(ExpressionHelpers.IntegerComparators["=="].Invoke(1, 1) == true);
		Assert.That(ExpressionHelpers.IntegerComparators["!="].Invoke(2, 1) == true);
		Assert.That(ExpressionHelpers.IntegerComparators["!="].Invoke(1, 1) == false);
	}

	[Test]
	public void FloatComparators()
	{
		Assert.That(ExpressionHelpers.FloatComparators[">"].Invoke(2, 1) == true);
		Assert.That(ExpressionHelpers.FloatComparators[">"].Invoke(1, 1) == false);
		Assert.That(ExpressionHelpers.FloatComparators[">="].Invoke(2, 1) == true);
		Assert.That(ExpressionHelpers.FloatComparators[">="].Invoke(1, 1) == true);
		Assert.That(ExpressionHelpers.FloatComparators["<"].Invoke(1, 2) == true);
		Assert.That(ExpressionHelpers.FloatComparators["<"].Invoke(1, 1) == false);
		Assert.That(ExpressionHelpers.FloatComparators["<="].Invoke(1, 2) == true);
		Assert.That(ExpressionHelpers.FloatComparators["<="].Invoke(1, 1) == true);
		Assert.That(ExpressionHelpers.FloatComparators["=="].Invoke(2, 1) == false);
		Assert.That(ExpressionHelpers.FloatComparators["=="].Invoke(1, 1) == true);
		Assert.That(ExpressionHelpers.FloatComparators["!="].Invoke(2, 1) == true);
		Assert.That(ExpressionHelpers.FloatComparators["!="].Invoke(1, 1) == false);
	}

	[Test]
	public void BoolComparators()
	{
		Assert.That(ExpressionHelpers.BoolComparators["=="].Invoke(true, true) == true);
		Assert.That(ExpressionHelpers.BoolComparators["=="].Invoke(false, true) == false);
		Assert.That(ExpressionHelpers.BoolComparators["!="].Invoke(true, true) == false);
		Assert.That(ExpressionHelpers.BoolComparators["!="].Invoke(false, true) == true);
	}

	[Test]
	public void ContextComparators()
	{
		var faction_1 = new Faction() { ID = 1 };
		var faction_2 = new Faction() { ID = 2 };

		Assert.That(ExpressionHelpers.ContextComparators["=="].Invoke(faction_1, faction_1) == true);
		Assert.That(ExpressionHelpers.ContextComparators["=="].Invoke(faction_1, faction_2) == false);
		Assert.That(ExpressionHelpers.ContextComparators["!="].Invoke(faction_1, faction_1) == false);
		Assert.That(ExpressionHelpers.ContextComparators["!="].Invoke(faction_1, faction_2) == true);
	}

	[Test]
	public void LocationComparators()
	{
		var location_1 = new Location() { TileIndex = 1 };
		var location_2 = new Location() { TileIndex = 2 };

		Assert.That(ExpressionHelpers.LocationComparators["=="].Invoke(location_1, location_1) == true);
		Assert.That(ExpressionHelpers.LocationComparators["=="].Invoke(location_1, location_2) == false);
		Assert.That(ExpressionHelpers.LocationComparators["!="].Invoke(location_1, location_1) == false);
		Assert.That(ExpressionHelpers.LocationComparators["!="].Invoke(location_1, location_2) == true);
	}

	[Test]
	public void TypeComparators()
	{
		var type_1 = typeof(Faction);
		var type_2 = typeof(Location);

		Assert.That(ExpressionHelpers.TypeComparators["=="].Invoke(type_1, type_1) == true);
		Assert.That(ExpressionHelpers.TypeComparators["=="].Invoke(type_1, type_2) == false);
		Assert.That(ExpressionHelpers.TypeComparators["!="].Invoke(type_1, type_1) == false);
		Assert.That(ExpressionHelpers.TypeComparators["!="].Invoke(type_1, type_2) == true);
	}

	[Test]
	public void IncidentWeightCalculate()
	{
		var expression_1 = new Expression<int>() { constValue = 2 };
		expression_1.ExpressionType = ExpressionType.Const;

		var incidentWeight = new IncidentWeight<Faction>();
		incidentWeight.baseWeight = 5;
		incidentWeight.Operation = "+";

		var faction = new Faction();

		var value = incidentWeight.CalculateWeight(faction);
		var expected = 5;

		Assert.That(value == expected);

		incidentWeight.expressions.Add(expression_1);
		value = incidentWeight.CalculateWeight(faction);
		expected = 7;

		Assert.That(value == expected);

		expression_1.constValue = -10;
		value = incidentWeight.CalculateWeight(faction);
		expected = 1;

		Assert.That(value == expected);
	}
}
