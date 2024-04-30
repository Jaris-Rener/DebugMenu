namespace DebugMenu
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public class DebugButtonAttribute : DebugControlAttribute
    {
        public DebugButtonAttribute() : base(null)
        {
        }

        public DebugButtonAttribute(string getMethodName) : base(getMethodName)
        {
        }

        public override IEnumerable<DebugFunction> GenerateFunctions(Type type, MethodInfo method)
        {
            var name = Name ?? method.ToDisplayName();
            yield return new DebugButton(name, type, method, GetMethodName);
        }
    }
}