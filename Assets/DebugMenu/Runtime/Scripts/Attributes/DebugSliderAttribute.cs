namespace DebugMenu
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using UnityEngine;

    public class DebugSliderAttribute : DebugControlAttribute
    {
        public Vector2 Range;

        public DebugSliderAttribute(string getMethodName, float min, float max)
            : base(getMethodName)
        {
            Range = new Vector2(min, max);
        }

        public override IEnumerable<DebugFunction> GenerateFunctions(Type type, MethodInfo method)
        {
            var name = Name ?? method.ToDisplayName();
            yield return new DebugSlider(name, type, method, GetMethodName, Range);
        }
    }
}