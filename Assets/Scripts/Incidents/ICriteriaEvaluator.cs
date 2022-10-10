using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Game.Incidents
{
	public interface ICriteriaEvaluator
	{
        Type Type { get; }
        bool Evaluate(IIncidentContext context, string propertyName);
	}

    public class IntegerEvaluator : ICriteriaEvaluator
    {
        private static Dictionary<string, Func<int, int, bool>> comparators = new Dictionary<string, Func<int, int, bool>>
        {
            {">", (a, b) => a > b },
            {">=", (a, b) => a >= b },
            {"<", (a, b) => a < b },
            {"<=", (a, b) => a <= b },
            {"==", (a, b) => a == b },
            {"!=", (a, b) => a != b }
        };

        [HideInInspector]
        public Type Type => typeof(int);

        private string comparator;

        [HorizontalGroup("Group 1", 150), HideLabel, ReadOnly]
        public readonly string propertyName;

        [ValueDropdown("GetComparatorNames"), OnValueChanged("SetComparatorType"), HorizontalGroup("Group 1", 50), HideLabel]
        public string Comparator;

        [HorizontalGroup("Group 1", 50), HideLabel, HideReferenceObjectPicker]
        public Expression<int> value;

        public IntegerEvaluator() { }

        public IntegerEvaluator(string propertyName)
        {
            this.propertyName = propertyName;
            value = new Expression<int>();
        }

        public IntegerEvaluator(string operation, int value)
        {
            comparator = operation;
            this.value.constValue = value;
        }

        public bool Evaluate(IIncidentContext context, string propertyName)
        {
            var propertyValue = (int)context.GetType().GetProperty(propertyName).GetValue(context);
            return comparators[Comparator].Invoke(propertyValue, value.constValue);
        }

        private List<string> GetComparatorNames()
        {
            return comparators.Keys.ToList();
        }

        private void SetComparatorType()
        {
            comparator = Comparator;
        }
    }

    public class FloatEvaluator : ICriteriaEvaluator
    {
        private static Dictionary<string, Func<float, float, bool>> comparators = new Dictionary<string, Func<float, float, bool>>
        {
            {">", (a, b) => a > b },
            {">=", (a, b) => a >= b },
            {"<", (a, b) => a < b },
            {"<=", (a, b) => a <= b },
            {"==", (a, b) => a == b },
            {"!=", (a, b) => a != b }
        };

        [HideInInspector]
        public Type Type => typeof(float);

        private string comparator;

        [HorizontalGroup("Group 1", 150), HideLabel, ReadOnly]
        public readonly string propertyName;

        [ValueDropdown("GetComparatorNames"), OnValueChanged("SetComparatorType"), HorizontalGroup("Group 1", 50), HideLabel]
        public string Comparator;

        [HorizontalGroup("Group 1", 50), HideLabel]
        public Expression<float> value;

        public FloatEvaluator() { }

        public FloatEvaluator(string propertyName)
		{
            this.propertyName = propertyName;
		}

        public FloatEvaluator(string operation, float value)
        {
            comparator = operation;
            this.value.constValue = value;
        }

        public bool Evaluate(IIncidentContext context, string propertyName)
        {
            var propertyValue = (float)context.GetType().GetProperty(propertyName).GetValue(context);
            return comparators[Comparator].Invoke(propertyValue, value.constValue);
        }

        private List<string> GetComparatorNames()
        {
            return comparators.Keys.ToList();
        }

        private void SetComparatorType()
        {
            comparator = Comparator;
        }
    }

    public class BoolEvaluator : ICriteriaEvaluator
    {
        private static Dictionary<string, Func<bool, bool, bool>> comparators = new Dictionary<string, Func<bool, bool, bool>>
        {
            {"==", (a, b) => a == b },
            {"!=", (a, b) => a != b }
        };

        [HideInInspector]
        public Type Type => typeof(bool);

        [HorizontalGroup("Group 1", 150), HideLabel, ReadOnly]
        public readonly string propertyName;

        [ValueDropdown("GetComparatorNames"), OnValueChanged("SetComparatorType"), HorizontalGroup("Group 1", 50), HideLabel]
        public string Comparator;

        [HorizontalGroup("Group 1", 50), HideLabel]
        public Expression<bool> value;

        public BoolEvaluator() { }

        public BoolEvaluator(string propertyName)
		{
            this.propertyName = propertyName;
		}

        public BoolEvaluator(string operation, bool value)
        {
            Comparator = operation;
            this.value.constValue = value;
        }

        public bool Evaluate(IIncidentContext context, string propertyName)
        {
            var propertyValue = (bool)context.GetType().GetProperty(propertyName).GetValue(context);
            return comparators[Comparator].Invoke(propertyValue, value.constValue);
        }

        private List<string> GetComparatorNames()
        {
            return comparators.Keys.ToList();
        }

        private void SetComparatorType()
        {
            //comparator = Comparator;
        }
    }
}