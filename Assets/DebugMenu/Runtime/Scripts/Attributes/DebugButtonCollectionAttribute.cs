namespace DebugMenu
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Reflection;

    public class DebugButtonCollectionAttribute : DebugButtonAttribute
    {
        public DebugButtonCollectionAttribute(string getMethodName) : base(getMethodName)
        {
        }

        public override IEnumerable<DebugFunction> GenerateFunctions(Type type, MethodInfo method)
        {
            var getMethod = type.GetMethod(GetMethodName);
            if (getMethod != null)
            {
                var items = (IEnumerable)getMethod?.Invoke(null, null); // TODO: Support non-static collections
                foreach (var item in items)
                {
                    var name = Name ?? method.ToDisplayName();
                    yield return new DebugButton($"{name} - {item}", type, method, GetMethodName, item);
                }
            }
        }
    }
}