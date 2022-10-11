using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Game.Incidents
{
	//THEN: try to make it so you can give the user a choice between a static value and
	//using a function to determine the value to check against. Even better if you can pass params to that fn.
    public enum ExpressionType { Const, Method };


	public class Expression<T>
	{
        private Dictionary<string, MethodInfo> methods;

        [HideInInspector]
        public bool hasNextOperator;

        public ExpressionType ExpressionType;

        [LabelText("Value"), ShowIf("ExpressionType", ExpressionType.Const)]
        public T constValue;

        [ValueDropdown("GetMethodNames"), ShowIf("ExpressionType", ExpressionType.Method), HideLabel]
        public string chosenMethod;

        [ShowIf("@this.hasNextOperator"), ValueDropdown("GetOperators"), HideLabel, HideReferenceObjectPicker]
        public string nextOperator;

        public Expression()
		{
            GetMethodInfo();
		}

        public T Value
		{
            get
			{
                if(ExpressionType == ExpressionType.Const)
				{
                    return constValue;
				}
                else
				{
                    return (T)methods[chosenMethod].Invoke(null, null);
				}
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

            return IntegerOperators.Keys.ToList();
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
    }
}