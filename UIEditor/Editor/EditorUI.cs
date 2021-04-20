using RenderingEngine.Datatypes;
using RenderingEngine.Datatypes.Geometric;
using RenderingEngine.Datatypes.UI;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.UI;
using RenderingEngine.UI.Components.AutoResizing;
using RenderingEngine.UI.Components.DataInput;
using RenderingEngine.UI.Components.Debugging;
using RenderingEngine.UI.Components.MouseInput;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using RenderingEngine.UI.Property;
using System;
using System.Collections.Generic;
using System.Text;
using UICodeGenerator.ComponentEditors;
using UICodeGenerator.DraggableRect;

namespace UICodeGenerator.Editor
{
    public class EditorUI : EntryPoint
    {
        UIZStack _root;
        UIEditorComponent _editorComponent;

        UIElement _mainWorkspace;
        UIElement _propertiesPanel;
        UIElement _uiView;
        UIElement _propertiesContainer;

        UIElement _rightclickMenu;


        UIElement _domRoot;
        UIDraggableRect _domRootRect;
        DraggableRectSelectedState _selectedState;


        public EditorUI() { }

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

#if DEBUG
            UICreator.Debug = true;
#endif

            _selectedState = new DraggableRectSelectedState();
            _selectedState.OnSelectionChanged += OnSelectionChanged;

            _domRoot = UIDraggableRect.CreateDraggableRect(_selectedState);
            _domRootRect = _domRoot.GetComponentOfType<UIDraggableRect>();

            InitRightclickMenu();
            InitRootUI();
        }

        public static UIElement CreateButton(string s)
        {
            return UICreator.CreateButton(s, "Consolas", 14, new Color4(0, 1), new Color4(1, 0), new Color4(0.5f), new Color4(0.75f));
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
        }


        private void OnSelectionTextEdited(string text)
        {
            if (_selectedState.SelectedRect != null)
                _selectedState.SelectedRect.Text = text;
        }

        private void OnSelectionNameEdited(string name)
        {
            if (_selectedState.SelectedRect != null)
                _selectedState.SelectedRect.Name = name;
        }

        private void OnSelectionFontEdited(string font)
        {
            if (_selectedState.SelectedRect != null)
                _selectedState.SelectedRect.FontName = font;
        }

        private void OnSelectionFontSize(long newSize)
        {
            if (_selectedState.SelectedRect != null)
                _selectedState.SelectedRect.TextSize = (int)newSize;
        }

        UIElementPropertyPair _unitSnapProperty;
        UIElementPropertyPair _normalizedSnapProperty;

        private void InitRootUI()
        {
            UIElement copyCodeButton;
            UIElement pasteCodeButton;

            UIElement copyPasteButtons = UICreator.CreateUIElement(
                new UILinearArrangement(vertical: true, reverse: true, 40, 10),
                new UIFitChildren(false, true)
            )
            .SetNormalizedAnchoringX(0,1)
            .SetAbsOffsetsX(10,10)
            .SetNormalizedPositionCenterY(0,0)
            .SetAbsPositionSizeY(10, 10)
            .AddChildren(
                copyCodeButton = CreateButton("Copy Code")
                ,
                pasteCodeButton = CreateButton("Paste Code")
            );

            _editorComponent = new UIEditorComponent(_selectedState);
            ComponentEditorUI<UIEditorComponent> uiEditorComponetUIEditor = new ComponentEditorUI<UIEditorComponent>(_editorComponent);
            UIElement editorComponentUI = uiEditorComponetUIEditor.Root;
            editorComponentUI.GetComponentOfType<UIRect>().InitialColor = new Color4(1, 0, 0, 0.5f);

            _root = new UIZStack();
            _root.AddComponent(new UIRectHitbox())
            .AddComponent(new UIMouseListener())
            .AddChildren(
                _mainWorkspace = UICreator.CreateUIElement()
                .AddComponent(new UIRect(new Color4(0, 0), new Color4(0, 1), 1))//remove later
                .AddChildren(
                    _uiView = UICreator.CreateUIElement(
                        new UIRect(new Color4(0, 0, 0, 0)),
                        new UIInverseStencil()
                    )
                    .SetNormalizedAnchoring(new Rect2D(0, 0, 0.75f, 1f))
                    .SetAbsoluteOffset(10)
                    .AddChildren(
                        _domRoot
                    )
                    ,
                    UICreator.CreateUIElement()
                    .AddComponent(new UIRect(new Color4(0,0), new Color4(0,1),1))//remove later
                    .SetNormalizedAnchoring(new Rect2D(0.75f, 0.0f, 1f, 1f))
                    .SetAbsoluteOffset(0)
                    .AddChildren(
                        _propertiesPanel = UICreator.CreatePanel(new Color4(1))
                        .SetAbsoluteOffset(new Rect2D(10,10,10,0))
                        .AddComponent(new UIDebugComponent())
                        .AddComponent(
                            new UIMultiEdgeSnapConstraint(
                                new UIEdgeSnapConstraint(_uiView, UIRectEdgeSnapEdge.Top, UIRectEdgeSnapEdge.Top),
                                new UIEdgeSnapConstraint(copyPasteButtons, UIRectEdgeSnapEdge.Bottom, UIRectEdgeSnapEdge.Top)
                            )
                        )
                        .AddChildren(
                            UICreator.CreateUIElement(
                                new UIText("Properties", new Color4(0), "Consolas", 16, VerticalAlignment.Center, HorizontalAlignment.Center)
                            )
                            .SetAbsOffsetsX(10, 10)
                            .SetNormalizedAnchoringX(0, 1)
                            .SetNormalizedPositionCenterY(1, 1)
                            .SetAbsPositionSizeY(-10, 50)
                            ,
                            _propertiesContainer = UICreator.CreateUIElement(
                                new UILinearArrangement(true, false, 30, 10)
                            )
                            .SetAbsOffsetsX(10, 10)
                            .SetNormalizedAnchoringX(0, 1)
                            .SetNormalizedPositionCenterY(1, 1)
                            .SetAbsPositionSizeY(-70, 50)
                            .AddChildren(
                                editorComponentUI
                                .SetNormalizedPositionCenterY(0, 0)
                            )
                        )
                        ,
                        copyPasteButtons
                    )
                )
            );

            _root.GetComponentOfType<UIMouseListener>().OnMousePressed += OnWindowClicked;

            copyCodeButton.GetComponentOfType<UIMouseListener>().OnMousePressed += OnCopyCodeButtonPressed;
            pasteCodeButton.GetComponentOfType<UIMouseListener>().OnMousePressed += OnPasteCodeButtonPressed;
        }

        private void OnSelectionTextSizeEdited(long size)
        {
            _selectedState.SelectedRect.TextSize = (int)size;
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
                new UILinearArrangement(true, false, 20, 10)
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

            if (Input.IsCtrlDown)
            {
                if (Input.IsKeyPressed(KeyCode.N))
                {
                    ResetRootElement();
                }
            }

            if (Input.IsShiftDown)
            {
                if (Input.IsKeyPressed(KeyCode.D))
                {
                    DuplicateSelected();
                }
            }

            _rightclickMenu.Update(deltaTime);

            _root.Update(deltaTime);
        }

        private void DuplicateSelected()
        {
            if (_selectedState.SelectedRect == null || _selectedState.SelectedRect == _domRootRect)
                return;

            UIElement duplicate = UIDraggableRect.DeepCopy(_selectedState.SelectedRect, offset: 10);

            _selectedState.SelectedRect.Parent.Parent.AddChild(duplicate);
        }

        private void ResetRootElement()
        {
            _uiView.RemoveAllChildren();
            _uiView.AddChild(
                _domRoot = UIDraggableRect.CreateDraggableRect(_selectedState)
            );
        }

        public override void Resize()
        {
            base.Resize();

            _rightclickMenu.Resize();
            _root.Resize();
        }
    }
}
