namespace Testing
{
    using System.Collections.Generic;
    using DebugMenu;
    using UnityEditor;
    using UnityEngine;

    public class TestItem
    {
        public TestItem(string name)
        {
            Name = name;
        }

        public string Name;

        public override string ToString() => Name;
    }

    public static class TestScript
    {
        public static readonly List<TestItem> Items = new()
        {
            new("Apple"),
            new("Orange"),
            new("Banana"),
            new("Peach"),
            new("Mango"),
        };

        [DebugButtonCollection(nameof(EnumerateItems), Name = "Alpha/Print Item Name")]
        public static void PrintItemName(TestItem item)
        {
            Debug.Log(item.Name);
        }

        public static IEnumerable<TestItem> EnumerateItems() => Items;

        [DebugButton(Name = "Alpha/Beta/Print")]
        public static void Print()
        {
            Debug.Log("print a word");
        }

        [DebugSlider(nameof(GetTimeScale), 0, 5, Name = "Alpha/Beta/Slider Sample")]
        public static void SetTimeScale(float val)
        {
            Time.timeScale = val;
        }

        [DebugToggle(nameof(GetMute), Name = "Alpha/Mute")]
        private static void SetMute(bool mute)
        {
            EditorUtility.audioMasterMute = mute;
        }

        private static bool GetMute()
        {
            return EditorUtility.audioMasterMute;
        }

        private static float GetTimeScale() => Time.timeScale;

        [DebugButton(Name = "Alpha/Beta/Erase")]
        public static void Erase()
        {
            Debug.Log("erase a word");
        }

        [DebugButton(Name = "Alpha/Gamma/Copy")]
        public static void Copy()
        {
            Debug.Log("copy a word");
        }

        [DebugButton(Name = "Beta/Alpha/Cut")]
        public static void Cut()
        {
            Debug.Log("cut a word");
        }

        [DebugButton(Name = "Test/Beta/Print")]
        public static void Print1()
        {
            Debug.Log("print a word");
        }

        [DebugButton(Name = "Test/Beta/Erase")]
        public static void Erase1()
        {
            Debug.Log("erase a word");
        }

        [DebugButton(Name = "Test/Gamma/Copy")]
        public static void Copy1()
        {
            Debug.Log("copy a word");
        }

        [DebugButton(Name = "Test/Alpha/Cut")]
        public static void Cut1()
        {
            Debug.Log("cut a word");
        }

        [DebugButton(Name = "Test/Beta/Print2")]
        public static void Print2()
        {
            Debug.Log("print a word");
        }

        [DebugButton(Name = "Test/Beta/Erase2")]
        public static void Erase2()
        {
            Debug.Log("erase a word");
        }

        [DebugButton(Name = "Test/Gamma/Copy2")]
        public static void Copy2()
        {
            Debug.Log("copy a word");
        }

        [DebugButton(Name = "Test/Alpha/Cut2")]
        public static void Cut2()
        {
            Debug.Log("cut a word");
        }
    }
}