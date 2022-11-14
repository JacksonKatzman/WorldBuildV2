﻿using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Game.Incidents
{
    public enum ExpressionType { Const, Method, Property, Range };


	public class Expression<T>
	{
        private Dictionary<string, MethodInfo> methods;
        private Dictionary<string, Type> properties;
        public Type ContextType { get; set; }

        [HideInInspector]
        public bool hasNextOperator;

        public ExpressionType ExpressionType;

        [LabelText("Value"), ShowIf("ExpressionType", ExpressionType.Const)]
        public T constValue;

        [ValueDropdown("GetMethodNames"), ShowIf("ExpressionType", ExpressionType.Method), HideLabel]
        public string chosenMethod;

        [ValueDropdown("GetPropertyNames"), ShowIf("ExpressionType", ExpressionType.Property), HideLabel]
        public string chosenProperty;

        [ShowIf("CanShowRange"), HideLabel]
        public IValueRange range;

        [ShowIf("RangeNotApplicable"), HideLabel, ReadOnly]
        public string rangeWarning = "Range not implemented for non integers!";

        [ShowIf("@this.hasNextOperator"), ValueDropdown("GetOperators"), HideLabel, HideReferenceObjectPicker]
        public string nextOperator;

        private bool CanShowRange => typeof(T) == typeof(int) && ExpressionType == ExpressionType.Range;
        private bool RangeNotApplicable => typeof(T) != typeof(int) && ExpressionType == ExpressionType.Range;

        public Expression()
		{
            GetMethodInfo();
            range = ValueRangeFactory.CreateValueRange<T>();
		}

        public Expression(Type contextType) : this()
		{
            ContextType = contextType;
		}

        public T GetValue(IIncidentContext context)
		{
            if(ExpressionType == ExpressionType.Method)
			{
                return (T)methods[chosenMethod].Invoke(null, null);
            }
            else if(ExpressionType == ExpressionType.Property)
			{
                return (T)context.GetType().GetProperty(chosenProperty).GetValue(context);
            }
            else if(ExpressionType == ExpressionType.Range && typeof(T) == typeof(int))
			{
                return ValueRangeFactory.FetchValue<T>(range);
			}
            else
			{
                return constValue;
			}
		}

        private void GetMethodInfo()
        {
            if (methods == null)
            {
                methods = new Dictionary<string, MethodInfo>();

                var methodInfos = typeof(ExpressionExtensions).GetMethods().Where(x => x.ReturnType == typeof(T) && x.DeclaringType == typeof(ExpressionExtensions)).ToList();

                methodInfos.ForEach(x => methods.Add(x.Name, x));
            }
        }

        private void GetPropertyList()
        {
            if (properties == null)
            {
                properties = new Dictionary<string, Type>();
            }
            if (ContextType != null)
            {
                var propertyInfo = ContextType.GetProperties();
                var interfacePropertyInfo = typeof(IIncidentContext).GetProperties();

                var validProperties = propertyInfo.Where(x => !interfacePropertyInfo.Any(y => x.Name == y.Name)).ToList();

                properties.Clear();

                validProperties.ForEach(x => properties.Add(x.Name, x.PropertyType));
            }
        }

        private IEnumerable<string> GetPropertyNames()
        {
            if (properties == null || properties.Count == 0)
            {
                GetPropertyList();
            }
            return properties.Keys.ToList();
        }

        private IEnumerable<string> GetMethodNames()
		{
            return methods.Keys.ToList();
		}

        private IEnumerable<string> GetOperators()
		{
            return ExpressionHelpers.GetOperatorNames<T>();
		}
	}

    public static class ExpressionExtensions
    {
        public static int FindSomeInteger() { return 0; }
        public static float FindSomeFloat() { return 0.0f; }
        public static bool FindSomeBool() { return false; }
    }

    public static class ExpressionHelpers
	{
        public static List<string> GetOperatorNames<T>()
		{
            var type = typeof(T);
            if(type == typeof(int))
			{
                return IntegerOperators.Keys.ToList();
			}
            else if(type == typeof(float))
			{
                return FloatOperators.Keys.ToList();
			}
			else
            {
                return BoolOperators.Keys.ToList();
            }
        }

        public static Dictionary<string, Func<int,int,int>> IntegerOperators = new Dictionary<string, Func<int, int, int>>
        {
            { "+", (a, b) => a + b },
            { "-", (a, b) => a - b },
            { "*", (a, b) => a * b },
            { "/", (a, b) => a / b },
            { "^", (a, b) => (int)System.Math.Pow(a, b) }
        };

        public static Dictionary<string, Func<int, int, bool>> IntegerComparators = new Dictionary<string, Func<int, int, bool>>
        {
            {">", (a, b) => a > b },
            {">=", (a, b) => a >= b },
            {"<", (a, b) => a < b },
            {"<=", (a, b) => a <= b },
            {"==", (a, b) => a == b },
            {"!=", (a, b) => a != b }
        };

        public static Dictionary<string, Func<float, float, float>> FloatOperators = new Dictionary<string, Func<float, float, float>>
        {
            { "+", (a, b) => a + b },
            { "-", (a, b) => a - b },
            { "*", (a, b) => a * b },
            { "/", (a, b) => a / b },
            { "^", (a, b) => (float)Mathf.Pow(a, b) }
        };

        public static Dictionary<string, Func<float, float, bool>> FloatComparators = new Dictionary<string, Func<float, float, bool>>
        {
            {">", (a, b) => a > b },
            {">=", (a, b) => a >= b },
            {"<", (a, b) => a < b },
            {"<=", (a, b) => a <= b },
            {"==", (a, b) => a == b },
            {"!=", (a, b) => a != b }
        };

        public static Dictionary<string, Func<bool, bool, bool>> BoolOperators = new Dictionary<string, Func<bool, bool, bool>>
        {
            { "&&", (a, b) => a && b },
            { "||", (a, b) => a || b }
        };

        public static Dictionary<string, Func<bool, bool, bool>> BoolComparators = new Dictionary<string, Func<bool, bool, bool>>
        {
            {"==", (a, b) => a == b },
            {"!=", (a, b) => a != b }
        };

        public static Dictionary<string, Func<IIncidentContext, IIncidentContext, bool>> ContextComparators = new Dictionary<string, Func<IIncidentContext, IIncidentContext, bool>>
        {
            {"==", (a, b) => a.ID == b.ID },
            {"!=", (a, b) => a.ID != b.ID }
        };

        public static Dictionary<string, Func<IIncidentContext, IIncidentContext, bool>> LocationComparators = new Dictionary<string, Func<IIncidentContext, IIncidentContext, bool>>
        {
            {"==", (a, b) => ((Location)a).TileIndex == ((Location)b).TileIndex },
            {"!=", (a, b) => ((Location)a).TileIndex != ((Location)b).TileIndex }
        };
    }
}