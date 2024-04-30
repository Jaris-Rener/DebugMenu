namespace DebugMenu
{
    using System;
    using UnityEngine;

    public abstract class DebugControlAttribute : Attribute
    {
        protected DebugControlAttribute(string getMethodName)
        {
            GetMethodName = getMethodName;
        }

        public readonly string GetMethodName;
        public string Name { get; set; }
    }

    public class DebugButtonAttribute : DebugControlAttribute
    {
        public DebugButtonAttribute() : base(null)
        {
        }

        public DebugButtonAttribute(string getMethodName) : base(getMethodName)
        {
        }
    }

    public class DebugToggleAttribute : DebugControlAttribute
    {
        public DebugToggleAttribute() : base(null)
        {
        }

        public DebugToggleAttribute(string getMethodName) : base(getMethodName)
        {
        }
    }

    public class DebugSliderAttribute : DebugControlAttribute
    {
        public Vector2 Range;

        public DebugSliderAttribute(string getMethodName, float min, float max)
            : base(getMethodName)
        {
            Range = new Vector2(min, max);
        }
    }
}