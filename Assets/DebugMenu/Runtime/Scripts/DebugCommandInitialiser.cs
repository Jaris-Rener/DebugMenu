namespace DebugMenu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using UnityEditor.Compilation;
    using Assembly = System.Reflection.Assembly;

    public class DebugCommandInitialiser
    {
        private readonly BindingFlags _attributeBindingFlags;
        private readonly DebugPageNode _rootNode;

        public DebugCommandInitialiser(BindingFlags bindingFlags, DebugPageNode rootNode)
        {
            _attributeBindingFlags = bindingFlags;
            _rootNode = rootNode;
        }

        public void BuildPages()
        {
            var playerAssemblies = CompilationPipeline.GetAssemblies(AssembliesType.Player);
            var assemblies = playerAssemblies.Where(x => !x.name.Contains("Unity")).Select(x => Assembly.Load(x.name));
            foreach (var asm in assemblies)
            {
                foreach (Type type in asm.GetTypes())
                    FindFunctions(type);
            }
        }

        private void FindFunctions(Type type)
        {
            foreach (var methodInfo in type.GetMethods(_attributeBindingFlags))
            {
                var cheatAttributes = methodInfo.GetCustomAttributes<DebugControlAttribute>();
                foreach (var attribute in cheatAttributes)
                {
                    RegisterFunction(type, methodInfo, attribute);
                }
            }
        }

        private void RegisterFunction(Type type, MethodInfo methodInfo, DebugControlAttribute attribute)
        {
            var name = attribute.Name ?? ToDisplayName(methodInfo.Name);
            DebugFunction function = attribute switch
            {
                DebugButtonAttribute btnAttr => new DebugButton(name, type, methodInfo, btnAttr.GetMethodName),
                DebugToggleAttribute tglAttr => new DebugToggle(name, type, methodInfo, tglAttr.GetMethodName),
                DebugSliderAttribute sldAttr => new DebugSlider(name, type, methodInfo, sldAttr.GetMethodName, sldAttr.Range),
                _ => throw new ArgumentOutOfRangeException(nameof(attribute), attribute, null)
            };

            var splitFunctionName = function.Path.Split("/");

            // Iterate over all but the last section of the path (e.g. One/Two)
            DebugPageNode currentNode = _rootNode;
            for (var i = 0; i < splitFunctionName.Length - 1; i++)
            {
                var part = splitFunctionName[i];
                currentNode = currentNode.AddOrGetNode(part);
            }

            currentNode.Data.RegisterFunction(function);
        }

        private static string ToDisplayName(string str)
            => Regex.Replace(str, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1", RegexOptions.Compiled).Trim();
    }
}