namespace DebugMenu
{
    using System;
    using System.Collections.Generic;

    [Serializable]
    public class DebugPageData
    {
        public IEnumerable<DebugFunction> Functions => _functions;
        private readonly List<DebugFunction> _functions = new();
        public string PageName;

        public DebugPageData(string name)
        {
            PageName = name;
        }

        public void RegisterFunction(DebugFunction function)
            => _functions.Add(function);

        public override string ToString() => PageName;
    }

    public class DebugPageNode
    {
        public DebugPageData Data { get; set; }
        public DebugPageNode Parent { get; private set; }
        public List<DebugPageNode> Children { get; } = new();

        public DebugPageNode(DebugPageData data, DebugPageNode parent)
        {
            Data = data;
            Parent = parent;
        }

        public DebugPageNode Add(string name)
        {
            var data = new DebugPageData(name);
            var node = new DebugPageNode(data, this);
            Children.Add(node);
            return node;
        }

        public DebugPageNode AddOrGetNode(string name)
        {
            var existing = Children.Find(x => x.Data.PageName == name);
            return existing ?? Add(name);
        }

        public void Traverse(DebugPageNode node, Action<DebugPageData> visitor)
        {
            visitor(node.Data);
            foreach (DebugPageNode child in node.Children)
                Traverse(child, visitor);
        }

        public override string ToString() => Data.ToString();
    }
}