namespace DebugMenu
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using UnityEngine.UIElements;

    public class DebugMenuUI : Singleton<DebugMenuUI>
    {
        [SerializeField] private UIDocument _uiDocument;

        [SerializeField] private VisualTreeAsset _uiPageButton;
        
        [SerializeField] private VisualTreeAsset _uiButtonTemplate;
        [SerializeField] private VisualTreeAsset _uiTextInputButtonTemplate;
        [SerializeField] private VisualTreeAsset _uiIntegerInputButtonTemplate;
        [SerializeField] private VisualTreeAsset _uiFloatInputButtonTemplate;
        [SerializeField] private VisualTreeAsset _uiEnumButtonTemplate;
        [SerializeField] private VisualTreeAsset _uiSliderTemplate;
        [SerializeField] private VisualTreeAsset _uiToggleTemplate;

        private readonly Dictionary<Type, object> _debugInstances = new();
        private readonly Dictionary<Type, VisualTreeAsset> _widgets = new();

        private ScrollView _uiContentArea;
        private ScrollView _uiPageScroll;
        private VisualElement _uiRoot;
        private VisualElement _uiMenuBar;
        private Button _closeBtn;
        private Button _minimiseBtn;
        private DebugCommandInitialiser _initialiser;

        private bool _ready;
        private readonly DebugPageNode _rootNode = new(new DebugPageData("Root"), null);

        public static void RegisterInstance<T>(T instance)
        {
            var type = instance.GetType();
            if (Instance != null && !Instance._debugInstances.TryAdd(type, instance))
                Debug.LogWarning($"Instance of type {type.Name} is already registered.");
        }

        private void Start()
        {
            _initialiser = new DebugCommandInitialiser(_rootNode);
            _initialiser.BuildPages();

            RegisterWidgets();

            _ready = true;
            Init();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.BackQuote))
                Toggle();
        }

        public void OnEnable()
        {
            if (_ready)
                Init();
        }

        private DebugPageNode _currentNode;

        private void Init()
        {
            _currentNode = _rootNode;
            PrepareUIDocument();

            SetupNode(_currentNode);
        }

        private void SetupNode(DebugPageNode node)
        {
            // Clear existing navigation buttons
            _uiPageScroll.Clear();

            // Add a button to go back up a level
            if (node.Parent != null)
                AddPageButton(node.Parent, $"<- Back | {node.Parent.Data.PageName}");

            // Add buttons for all the child pages
            foreach (var child in node.Children)
                AddPageButton(child);

            // TODO: Separate view & logic into different scripts
            // Add the functions for this node into the main content area
            SetupNodeFunctions(node);
        }

        private void SetupNodeFunctions(DebugPageNode node)
        {
            // Clear old content
            _uiContentArea.Clear();

            foreach (var function in node.Data.Functions)
            {
                // TODO: Could the DebugFunction class handle its own setup given the content container?
                switch (function)
                {
                    case DebugSlider sldFunc:
                        SetupSlider(sldFunc);
                        break;
                    case DebugButton btnFunc:
                        SetupButton(btnFunc);
                        break;
                    case DebugToggle tglFnc:
                        SetupToggle(tglFnc);
                        break;
                }
            }
        }

        private void AddPageButton(DebugPageNode node, string btnName = null)
        {
            var instance = _uiPageButton.Instantiate();
            var button = instance.Q<Button>();
            button.name = button.text = btnName ?? node.Data.PageName;
            button.clicked += OnClicked;
            _uiPageScroll.Add(button);
            return;

            void OnClicked()
            {
                _currentNode = node;
                SetupNode(node);
            }
        }

        private void RegisterWidgets()
        {
            _widgets.Add(typeof(int), _uiIntegerInputButtonTemplate);
            _widgets.Add(typeof(float), _uiFloatInputButtonTemplate);
            _widgets.Add(typeof(string), _uiTextInputButtonTemplate);
            _widgets.Add(typeof(Enum), _uiEnumButtonTemplate);
        }

        private void PrepareUIDocument()
        {
            _uiRoot = _uiDocument.rootVisualElement;
            _uiContentArea = _uiRoot.Q<ScrollView>("ContentArea");
            _uiPageScroll = _uiRoot.Q<ScrollView>("PageScroll");
            _uiMenuBar = _uiRoot.Q<VisualElement>("MenuBar");

            _uiRoot.AddManipulator(new WindowDragger(_uiMenuBar));

            _closeBtn = _uiRoot.Q<Button>("Close");
            _closeBtn.clicked += Close;
            _minimiseBtn = _uiRoot.Q<Button>("Minimise");
            _minimiseBtn.clicked += Minimise;
        }

        private void Close()
        {
            _uiDocument.enabled = false;
        }

        private void Minimise()
        {
            _uiDocument.enabled = false;
        }

        private void Toggle()
        {
            _uiDocument.enabled = !_uiDocument.enabled;
            if (_uiDocument.enabled)
                Init();
        }

        private void SetupButton(DebugButton function)
        {
            _debugInstances.TryGetValue(function.Type, out var typeInstance);

            VisualElement instance = null;
            var parameters = function.Method.GetParameters();
            Func<object[]> getParams = null;

            // If we don't have objects for our parameters, we might need an input of some kind
            var unresolvedParams = parameters.Length - function.Parameters.Length;

            // TODO: Improve how we select field types
            if (unresolvedParams == 0)
            {
                instance = _uiButtonTemplate.Instantiate();
            }
            else if (unresolvedParams == 1)
            {
                var parameterType = parameters[0].ParameterType;
                var (_, template) = _widgets.FirstOrDefault(x => x.Key.IsAssignableFrom(parameterType));
                if (template != null)
                    instance = template.Instantiate();

                if (parameterType == typeof(string))
                {
                    var input = instance.Q<TextField>();
                    getParams = () => new object[] { input.value };
                }
                else if (parameterType == typeof(float))
                {
                    var input = instance.Q<FloatField>();
                    getParams = () => new object[] { input.value };
                }
                else if (parameterType == typeof(int))
                {
                    var input = instance.Q<IntegerField>();
                    getParams = () => new object[] { input.value };
                }
                else if (parameterType.IsEnum)
                {
                    var input = instance.Q<EnumField>();
                    input.Init(Enum.ToObject(parameterType, 0) as Enum);
                    getParams = () => new object[] { input.value };
                }
            }

            if (instance == null)
                return;

            _uiContentArea.Add(instance);
            instance.name = function.FuncName;

            var button = instance.Q<Button>();
            if (button == null)
                return;

            button.text = function.FuncName;
            var runParams = (getParams?.Invoke() ?? Array.Empty<object>()).Concat(function.Parameters);
            button.clicked += () => RunCommand(function, typeInstance, runParams.ToArray());
        }

        private void SetupSlider(DebugSlider function)
        {
            _debugInstances.TryGetValue(function.Type, out var typeInstance);

            var instance = _uiSliderTemplate.Instantiate();
            if (instance == null)
                return;

            _uiContentArea.Add(instance);
            instance.name = function.FuncName;

            var slider = instance.Q<Slider>();
            if (slider == null)
                return;

            if (!string.IsNullOrEmpty(function.GetMethodName))
            {
                var getMethod = function.Type.GetMethod(function.GetMethodName, DebugMenuUtil.BindingFlags);
                slider.lowValue = function.Range.x;
                slider.highValue = function.Range.y;
                if (getMethod != null)
                    slider.value = (float)getMethod.Invoke(typeInstance, null); // TODO: Watch for changes outside of this control?
            }

            slider.label = function.FuncName;
            slider.RegisterValueChangedCallback(val => RunCommand(function, typeInstance, val.newValue));
        }

        private void SetupToggle(DebugToggle function)
        {
            _debugInstances.TryGetValue(function.Type, out var typeInstance);

            var instance = _uiToggleTemplate.Instantiate();
            if (instance == null)
                return;

            _uiContentArea.Add(instance);
            instance.name = function.FuncName;

            var toggle = instance.Q<Toggle>();
            if (toggle == null)
                return;

            if (!string.IsNullOrEmpty(function.GetMethodName))
            {
                var getMethod = function.Type.GetMethod(function.GetMethodName, DebugMenuUtil.BindingFlags);
                if (getMethod != null)
                    toggle.value = (bool)getMethod.Invoke(typeInstance, null); // TODO: Watch for changes outside of this control?
            }

            toggle.label = function.FuncName;
            toggle.RegisterValueChangedCallback(val => RunCommand(function, typeInstance, val.newValue));
        }

        private void RunCommand(DebugFunction function, object typeInstance, params object[] parameters)
        {
            if (!function.Method.IsStatic && typeInstance == null)
            {
                Debug.LogWarning($"Cannot invoke non-static method {function.Method.Name} without a registered instance of type {function.Type.Name}.");
                return;
            }

            function.Method?.Invoke(typeInstance, parameters);
        }
    }
}