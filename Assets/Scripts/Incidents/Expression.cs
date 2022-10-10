using Sirenix.OdinInspector;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Game.Incidents
{
	//THEN: try to make it so you can give the user a choice between a static value and
	//using a function to determine the value to check against. Even better if you can pass params to that fn.
    public enum ExpressionType { Const, Method };


	public class Expression<T>
	{
        private Dictionary<string, MethodInfo> methods;

        public ExpressionType ExpressionType;

        [LabelText("Value"), ShowIf("ExpressionType", ExpressionType.Const)]
        public T constValue;

        [ValueDropdown("GetMethodNames"), ShowIf("ExpressionType", ExpressionType.Method), HideLabel]
        public string chosenMethod;

        public Expression()
		{
            GetMethodInfo();
		}

        private void GetMethodInfo()
        {
            if (methods == null)
            {
                methods = new Dictionary<string, MethodInfo>();

                var methodInfos = typeof(ExpressionHelpers).GetMethods().Where(x => x.ReturnType == typeof(T) && x.DeclaringType == typeof(ExpressionHelpers)).ToList();

                methodInfos.ForEach(x => methods.Add(x.Name, x));
            }
        }

        private IEnumerable<string> GetMethodNames()
		{
            return methods.Keys.ToList();
		}
	}

    public static class ExpressionHelpers
    {
        public static int FindSomeInteger() { return 0; }
        public static float FindSomeFloat() { return 0.0f; }
        public static bool FindSomeBool() { return false; }
    }
}