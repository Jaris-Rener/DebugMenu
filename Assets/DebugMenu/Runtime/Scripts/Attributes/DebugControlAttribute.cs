namespace DebugMenu
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    public abstract class DebugControlAttribute : Attribute
    {
        protected DebugControlAttribute(string getMethodName)
        {
            GetMethodName = getMethodName;
        }

        public readonly string GetMethodName;
        public string Name { get; set; }

        public abstract IEnumerable<DebugFunction> GenerateFunctions(Type type, MethodInfo method);
    }
}