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

        UIElementEditor _domRoot;
        DraggableRectSelectedState _selectedState;

        ComponentEditorListUI _componentList;


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

            _domRoot = UIElementEditor.CreateDraggableRect(_selectedState);

            InitRightclickMenu();
            InitRootUI();
        }

        public static UIElement CreateButton(string s)
        {
            return UICreator.CreateButton(s, "Consolas", 14, new Color4(0, 1), new Color4(1, 0), new Color4(0.5f), new Color4(0.75f));
        }

        private void OnSelectionChanged(UIElementEditor obj)
        {
            Console.WriteLine("Selection changed");
        }

        private void InitRootUI()
        {
            UIElement copyCodeButton;
            UIElement pasteCodeButton;

            UIElement copyPasteButtons = UICreator.CreateUIElement(
                new UIRect(new Color4(0, 0), new Color4(0, 1), 1),
                new UILinearArrangement(vertical: true, reverse: true, 40, 10)
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

            _componentList = new ComponentEditorListUI(_selectedState);

            _root = new UIZStack();
            _root.AddComponent(new UIRectHitbox())
            .AddComponent(_editorComponent)
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
                    .AddComponent(
                        new UIRect(new Color4(0,0), new Color4(0,1),1)
                    )//remove later
                    .SetNormalizedAnchoring(new Rect2D(0.75f, 0.0f, 1f, 1f))
                    .SetAbsoluteOffset(0)
                    .AddChildren(
                        _propertiesPanel = UICreator.CreatePanel(new Color4(0,0))
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
                                new UIRect(new Color4(0,1,0,0.5f)),
                                new UIInverseStencil(),
                                new UIEdgeSnapConstraint(editorComponentUI, UIRectEdgeSnapEdge.Bottom, UIRectEdgeSnapEdge.Top)
                            )
                            .SetAbsoluteOffset(new Rect2D(10,10,10,70))
                            .AddChildren(
                                _componentList.Root
                                .SetNormalizedPositionCenterY(1,1)
                                .SetAbsPositionSizeY(-10,10)
                                .SetAbsOffsetsX(10,10)
                            )
                            ,
                            editorComponentUI
                            .SetNormalizedPositionCenterY(0, 0)
                            .SetAbsPositionSizeY(10, 10)
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
            Window.ClipboardString = codeGenerator.GenerateCode(_domRoot);
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
            if (_selectedState.SelectedEditorRect == null)
                return;

            Console.WriteLine("This method is useless now, hopefully");
        }

        private void OnTogglePosSizeYButtonClicked()
        {
            if (_selectedState.SelectedEditorRect == null)
                return;

            Console.WriteLine("This method is useless now, hopefully");
        }


        private void OnDeleteButtonClicked()
        {
            if (_selectedState.SelectedEditorRect == null)
                return;

            if (_selectedState.SelectedEditorRect == _domRoot)
                return;

            _selectedState.SelectedEditorRect.Parent.RemoveChild(_selectedState.SelectedEditorRect);
        }

        private void OnNewButtonClicked()
        {
            if (_selectedState.SelectedEditorRect == null)
                return;

            UIElementEditor newRect;
            _selectedState.SelectedEditorRect.AddChild(
                newRect = UIElementEditor.CreateDraggableRect(_selectedState)
            );

            newRect.SetAbsoluteOffset(20f);

            _selectedState.SelectedEditorRect = newRect;
        }

        private void OnWindowClicked()
        {
            Console.WriteLine("Window clicked");

            _selectedState.SelectedEditorRect = null;
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
                    if (_selectedState.SelectedEditorRect == null)
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
            var selected = _selectedState.SelectedEditorRect;
            if (selected == null || selected == _domRoot)
                return;

            UIElement duplicate = selected.DeepCopy();

            selected.Parent.AddChild(duplicate);
        }

        private void ResetRootElement()
        {
            _uiView.RemoveAllChildren();
            _uiView.AddChild(
                _domRoot = UIElementEditor.CreateDraggableRect(_selectedState)
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
