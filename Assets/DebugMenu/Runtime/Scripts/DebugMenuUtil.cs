namespace DebugMenu
{
    using System.Reflection;
    using System.Text.RegularExpressions;

    public static class DebugMenuUtil
    {
        public static string ToDisplayName(this MethodInfo methodInfo)
            => Regex.Replace(methodInfo.Name, "(?<!^)([A-Z][a-z]|(?<=[a-z])[A-Z])", " $1", RegexOptions.Compiled).Trim();
    }
}