namespace DebugMenu
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using UnityEngine;

    public abstract class DebugFunction
    {
        public DebugFunction(string name, Type type, MethodInfo method, string getMethodName)
        {
            GetMethodName = getMethodName;
            Path = name;
            Type = type;
            Method = method;
        }

        public string FuncName
        {
            get
            {
                // Get just the last part of the function name path (e.g. One/Two/Three returns Three)
                var funcMatches = Regex.Match(Path, @"([^\/]+)(?=[^\/]*\/?$)");
                return funcMatches.Groups.FirstOrDefault()?.Value;
            }
        }

        public readonly string GetMethodName;
        public readonly string Path;
        public readonly Type Type;
        public readonly MethodInfo Method;

        public override string ToString() => Path;
    }

    public class DebugButton : DebugFunction
    {
        public DebugButton(string name, Type type, MethodInfo method, string getMethodName)
            : base(name, type, method, getMethodName)
        {
        }
    }

    public class DebugToggle : DebugFunction
    {
        public DebugToggle(string name, Type type, MethodInfo method, string getMethodName)
            : base(name, type, method, getMethodName)
        {
        }
    }

    public class DebugSlider : DebugFunction
    {
        public readonly Vector2 Range;

        public DebugSlider(string name, Type type, MethodInfo method, string getMethodName, Vector2 range)
            : base(name, type, method, getMethodName)
        {
            Range = range;
        }
    }
}