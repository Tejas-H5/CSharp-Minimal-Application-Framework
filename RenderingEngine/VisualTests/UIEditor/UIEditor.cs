using RenderingEngine.Datatypes;
using RenderingEngine.Datatypes.Geometric;
using RenderingEngine.Datatypes.UI;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.UI;
using RenderingEngine.UI.Components.AutoResizing;
using RenderingEngine.UI.Components.DataInput;
using RenderingEngine.UI.Components.MouseInput;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using RenderingEngine.UI.Properties;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.VisualTests.UIEditor
{
    class NamePropertyPairUI<T>
    {
        public UIElement Root;
        public readonly Property<T> Property;

        public NamePropertyPairUI(UIElement root, Property<T> property)
        {
            Root = root;
            Property = property;
        }
    }

    public class UIEditor : EntryPoint
    {
        UIZStack _root;
        UIElement _mainWorkspace;
        UIElement _propertiesPanel;
        UIElement _uiView;

        UIElement _rightclickMenu;


        UIElement _domRoot;
        UIDraggableRect _domRootRect;
        DraggableRectSelectedState _selectedState;


        UIElement CreatePanel()
        {
            return UICreator.CreatePanel(new Color4(0, 0.0f));
        }

        enum PropertyType
        {
            Float,
            Int,
        };


        public override void Start()
        {
            Window.Maximize();
            Window.Title = "UI editor";
            Window.UpdateFrequency = 120.0f;
            Window.RenderFrequency = 120.0f;

            CTX.SetClearColor(1, 1, 1, 1);

            UICreator.Debug = true;
            _selectedState = new DraggableRectSelectedState();
            _selectedState.OnSelectionChanged += OnSelectionChanged;

            _domRoot = UIDraggableRect.CreateDraggableRect(_selectedState);
            _domRootRect = _domRoot.GetComponentOfType<UIDraggableRect>();

            InitRightclickMenu();
            InitRootUI();
        }

        private UIElement CreateButton(string s)
        {
            return UICreator.CreateButton(s, "Consolas", 14, new Color4(0, 1), new Color4(1, 0), new Color4(0.5f), new Color4(0.75f));
        }

        private UIElement CreateEmptyPropertyElement()
        {
            return UICreator.CreateUIElement(
                new UIRect(new Color4(0, 0), new Color4(0, 1), 1),
                new UIRectHitbox(),
                new UIMouseListener(),
                new UIMouseFeedback(new Color4(0.5f), new Color4(0.75f))
            )
            .AddChild(
                UICreator.CreateUIElement(
                    new UIText("", new Color4(0f), "Consolas", 14, VerticalAlignment.Center, HorizontalAlignment.Right)
                )
                .SetAbsoluteOffset(10)
            );
        }

        public FloatProperty CreateFloatProperty(Action<double> callback)
        {
            var prop = new FloatProperty();
            prop.OnDataChanged += callback;
            return prop;
        }

        public StringProperty CreateStringProperty(Action<string> callback)
        {
            var prop = new StringProperty("");
            prop.OnDataChanged += callback;
            return prop;
        }


        private NamePropertyPairUI<string> CreateStringPropertyElement(StringProperty prop)
        {
            var stringInputComponent = new UITextStringInput(prop, "Enter text", false, false);

            UIElement root = CreateEmptyPropertyElement()
                .AddComponent(stringInputComponent);

            return new NamePropertyPairUI<string>(root, stringInputComponent.StringProperty);
        }

        
        private NamePropertyPairUI<double> CreateFloatPropertyElement(FloatProperty prop)
        {
            var floatInputComponent = new UITextFloatInput(prop);
            UIElement root = CreateEmptyPropertyElement()
                .AddComponent(floatInputComponent);

            return new NamePropertyPairUI<double>(root, floatInputComponent.FloatProperty);
        }

        private NamePropertyPairUI<T> CreateNamePropPair<T>(string name, NamePropertyPairUI<T> propertyElement)
        {
            UIElement left, right;

            UIElement newRoot = UICreator.CreateUIElement(
                new UIRect(new Color4(0, 0), new Color4(0, 1), 1)
                )
                .SetNormalizedAnchoring(new Rect2D(0, 0, 1, 1))
                .SetAbsoluteOffset(10)
                .AddChildren(
                    left = UICreator.CreateUIElement(
                        
                    )
                    .SetNormalizedAnchoring(new Rect2D(0,0,0.5f,1f))
                    .SetAbsoluteOffset(10)
                    .AddChild(
                        UICreator.CreateUIElement(
                            new UIText(name, new Color4(0, 1), "Consolas", 12, VerticalAlignment.Center, HorizontalAlignment.Left)
                        )
                    )
                    ,
                    right = propertyElement.Root
                    .SetNormalizedAnchoring(new Rect2D(0.5f, 0, 1f, 1f))
                );

            propertyElement.Root = newRoot;
            return propertyElement;
        }

        void OnUnitSnapChanged(double value)
        {
            _selectedState.DimensionSnap = (float)value;
        }

        void OnAnchoredSnapChanged(double value)
        {
            Console.WriteLine(value);
            _selectedState.AnchorSnap = (float)value;
        }

        private void OnSelectionChanged(UIDraggableRect obj)
        {
            Console.WriteLine("Selection changed");
            _rectNameProperty.Property.Value = obj.Name;
            _rectTextProperty.Property.Value = obj.Text;
        }

        NamePropertyPairUI<string> _rectNameProperty;
        NamePropertyPairUI<string> _rectTextProperty;

        private void OnSelectionTextEdited(string text)
        {
            if (_selectedState.SelectedRect != null)
                _selectedState.SelectedRect.Text = text;
        }

        private void OnSelectionNameEdited(string name)
        {
            if(_selectedState.SelectedRect != null)
                _selectedState.SelectedRect.Name = name;
        }

        NamePropertyPairUI<double> _unitSnapProperty;
        NamePropertyPairUI<double> _normalizedSnapProperty;

        private void InitRootUI()
        {
            UIElement copyCodeButton;
            UIElement pasteLayoutButton;

            _unitSnapProperty = CreateNamePropPair("Unit Snap", CreateFloatPropertyElement(
                CreateFloatProperty(OnUnitSnapChanged)
            ));
            _normalizedSnapProperty = CreateNamePropPair("Anchor Snap", CreateFloatPropertyElement(
                CreateFloatProperty(OnAnchoredSnapChanged)
            ));

            _rectNameProperty = CreateNamePropPair("Name", CreateStringPropertyElement(
                CreateStringProperty(OnSelectionNameEdited)
            ));

            _rectTextProperty = CreateNamePropPair("Text", CreateStringPropertyElement(
                CreateStringProperty(OnSelectionTextEdited)
            ));

            _root = new UIZStack();
            _root.AddComponent(new UIRectHitbox())
            .AddComponent(new UIMouseListener())
            .AddChildren(
                _mainWorkspace = UICreator.CreateUIElement()
                .AddChildren(
                    _uiView = UICreator.CreateUIElement(
                        new UIRect(new Color4(0,0,0,0)),
                        new UIInverseStencil()
                    )
                    .SetNormalizedAnchoring(new Rect2D(0, 0, 0.75f, 1f))
                    .SetAbsoluteOffset(10)
                    .AddChildren(
                        _domRoot
                    )
                    ,
                    UICreator.CreateUIElement()
                    .SetNormalizedAnchoring(new Rect2D(0.75f, 0.0f, 1f, 1f))
                    .SetAbsoluteOffset(10)
                    .AddChildren(
                        UICreator.CreateUIElement(
                            new UILinearArrangement(true, true, 40, 10)
                        )
                        .AddChildren(
                            _unitSnapProperty.Root
                            ,
                            _normalizedSnapProperty.Root
                            ,
                            copyCodeButton = CreateButton("Copy Code")
                            ,
                            pasteLayoutButton = CreateButton("Paste Code")
                        )
                        ,
                        _propertiesPanel = UICreator.CreatePanel(new Color4(1))
                        .SetAbsoluteOffset(10)
                        .AddComponent(
                            new UIMultiEdgeSnapConstraint(
                                new UIEdgeSnapConstraint(_uiView, UIRectEdgeSnapEdge.Top, UIRectEdgeSnapEdge.Top),
                                new UIEdgeSnapConstraint(pasteLayoutButton, UIRectEdgeSnapEdge.Bottom, UIRectEdgeSnapEdge.Top)
                            )
                        )
                        .AddChildren(
                            UICreator.CreateUIElement(
                                new UILinearArrangement(true, false, 30, 10)
                            )
                            .SetAbsOffsetsX(10, 10)
                            .SetNormalizedAnchoringX(0, 1)
                            .SetNormalizedPositionCenterY(1, 1)
                            .SetAbsPositionSizeY(-10, 50)
                            .AddChildren(
                                UICreator.CreateUIElement(
                                    new UIText("Properties", new Color4(0), "Consolas", 16, VerticalAlignment.Center, HorizontalAlignment.Center)
                                ),
                                _rectNameProperty.Root,
                                _rectTextProperty.Root
                                //CreateColorPropPair("Text(?)", CreateColorPropertyElement(OnSelectionColorEdited))
                            )
                        )
                    )
                //_propertiesView = new UILinearArrangement(vertical: true, reverse: false, padding: 10f)
                )
            );

            _root.GetComponentOfType<UIMouseListener>().OnMousePressed += OnWindowClicked;

            copyCodeButton.GetComponentOfType<UIMouseListener>().OnMousePressed += OnCopyCodeButtonPressed;
            pasteLayoutButton.GetComponentOfType<UIMouseListener>().OnMousePressed += OnPasteCodeButtonPressed;
        }

        private void OnPasteCodeButtonPressed()
        {
            _uiView.RemoveAllChildren();
            _uiView.AddChild(
                _domRoot = codeParser.CreateDraggableRectsFromString(Window.ClipboardString, _selectedState)
            );
        }

        UIDraggableRectTreeCodeGenerator codeGenerator = new UIDraggableRectTreeCodeGenerator();
        UIDraggableRectTreeParser codeParser = new UIDraggableRectTreeParser();

        private void OnCopyCodeButtonPressed()
        {
            Window.ClipboardString = codeGenerator.GenerateCode(_domRoot.GetComponentOfType<UIDraggableRect>());
        }

        private void InitRightclickMenu()
        {
            UIElement createButton;
            UIElement deleteButton;

            _rightclickMenu = UICreator.CreateUIElement(
                new UILinearArrangement(true, false, 20,10)
            )
            .SetNormalizedPositionCenter(0, 0, 0, 0)
            .AddChildren(
                createButton = UICreator.CreateButton("Add"),
                deleteButton = UICreator.CreateButton("Delete")
            );

            _rightclickMenu.IsVisible = false;

            createButton.GetComponentOfType<UIMouseListener>().OnMousePressed += OnNewButtonClicked;
            deleteButton.GetComponentOfType<UIMouseListener>().OnMousePressed += OnDeleteButtonClicked;
        }

        private void OnTogglePosSizeXButtonClicked()
        {
            if (_selectedState.SelectedRect == null)
                return;

            Console.WriteLine("This method is useless now, hopefully");
        }

        private void OnTogglePosSizeYButtonClicked()
        {
            if (_selectedState.SelectedRect == null)
                return;

            Console.WriteLine("This method is useless now, hopefully");
        }


        private void OnDeleteButtonClicked()
        {
            if (_selectedState.SelectedRect == null)
                return;

            if (_selectedState.SelectedRect == _domRootRect)
                return;

            UIElement node = _selectedState.SelectedRect.Parent;

            _selectedState.SelectedRect = node.Parent.GetComponentOfType<UIDraggableRect>();

            node.Parent.RemoveChild(node);

        }

        private void OnNewButtonClicked()
        {
            if (_selectedState.SelectedRect == null)
                return;

            UIElement newRect;
            _selectedState.SelectedRect.Parent.AddChild(
                newRect = UIDraggableRect.CreateDraggableRect(_selectedState)
                .SetAbsoluteOffset(20f)
            );

            _selectedState.SelectedRect = newRect.GetComponentOfType<UIDraggableRect>();
        }

        private void OnWindowClicked()
        {
            Console.WriteLine("Window clicked");

            _selectedState.SelectedRect = null;
        }

        public override void Render(double deltaTime)
        {
            _root.DrawIfVisible(deltaTime);
            _rightclickMenu.DrawIfVisible(deltaTime);
        }


        public override void Update(double deltaTime)
        {
            if (Input.IsMouseClickedAny)
            {
                if (Input.IsMouseClicked(MouseButton.Right))
                {
                    if (_selectedState.SelectedRect == null)
                        return;


                    _rightclickMenu.IsVisible = true;
                    _rightclickMenu.SetAbsPositionSize(Input.MouseX, Input.MouseY, _rightclickMenu.Rect.Width, _rightclickMenu.Rect.Height);
                }
                else
                {
                    _rightclickMenu.IsVisibleNextFrame = false;
                }
            }

            if (Input.IsKeyPressed(KeyCode.N))
            {
                if (Input.IsCtrlDown)
                {
                    _uiView.RemoveAllChildren();
                    _uiView.AddChild(
                        _domRoot = UIDraggableRect.CreateDraggableRect(_selectedState)
                    );
                }
            }

            _rightclickMenu.Update(deltaTime);

            _root.Update(deltaTime);
        }

        public override void Resize()
        {
            base.Resize();

            _rightclickMenu.Resize();
            _root.Resize();
        }
    }
}
