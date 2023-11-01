using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game.Incidents
{
	public class ListContainsContextTagEvaluator<T> : ICriteriaEvaluator where T: IContextTag
    {
        public static Dictionary<string, Func<List<T>, T, bool>> ListContextTagComparators = new Dictionary<string, Func<List<T>, T, bool>>
        {
            {"CONTAINS", (a, b) => a.Contains(b) },
            {"! CONTAINS", (a, b) => !a.Contains(b) }
        };

        [HorizontalGroup("Group 1", 150), HideLabel, ReadOnly]
        public string propertyName;

        [ValueDropdown("GetComparatorNames"), OnValueChanged("SetComparatorType"), HorizontalGroup("Group 1", 200), HideLabel]
        public string Comparator;

        [HorizontalGroup("Group 1", 400), HideLabel]
        public T tag;

        private string comparator;

        public Type ContextType { get; set; }
        public Type Type => typeof(List<T>);


        public ListContainsContextTagEvaluator(string propertyName, Type contextType)
        {
            ContextType = contextType;
            this.propertyName = propertyName;
            tag = (T)Activator.CreateInstance(typeof(T));
            Setup();
        }

        public void Setup()
        {

        }

		public bool Evaluate(IIncidentContext context, string propertyName, IIncidentContext parentContext = null)
		{
            var propertyValue = (List<T>)context.GetType().GetProperty(propertyName).GetValue(context);
            var result = ListContextTagComparators[Comparator].Invoke(propertyValue, tag);
            return result;
        }
        private List<string> GetComparatorNames()
        {
            return ListContextTagComparators.Keys.ToList();
        }

        private void SetComparatorType()
        {
            comparator = Comparator;
        }
    }
}