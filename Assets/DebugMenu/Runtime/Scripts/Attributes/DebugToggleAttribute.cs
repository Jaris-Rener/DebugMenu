namespace DebugMenu
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class DebugToggleAttribute : DebugControlAttribute
    {
        public DebugToggleAttribute(string getMethodName) : base(getMethodName)
        {
        }

        public override IEnumerable<DebugFunction> GenerateFunctions(Type type, MethodInfo method)
        {
            var name = Name ?? method.ToDisplayName();
            yield return new DebugToggle(name, type, method, GetMethodName);
        }
    }
}