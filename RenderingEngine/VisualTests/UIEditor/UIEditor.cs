using RenderingEngine.Datatypes;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.UI;
using RenderingEngine.UI.Components;
using RenderingEngine.UI.Core;
using System;

namespace RenderingEngine.VisualTests.UIEditor
{
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

        public override void Start()
        {
            Window.Maximize();
            Window.Title = "UI editor";
            Window.UpdateFrequency = 120.0f;
            Window.RenderFrequency = 120.0f;

            CTX.SetClearColor(1, 1, 1, 1);
            CTX.SetCurrentFont("Consolas", 12);

            UICreator.Debug = true;
            _selectedState = new DraggableRectSelectedState();
            _selectedState.OnSelectionChanged += OnSelectionChanged;

            _domRoot = UIDraggableRect.CreateDraggableRect(_selectedState);
            _domRootRect = _domRoot.GetComponentOfType<UIDraggableRect>();

            InitRightclickMenu();
            InitRootUI();
        }

        private void OnSelectionChanged(UIDraggableRect obj)
        {
            //Update the text fields and whatnot we will have later


        }

        private void InitRootUI()
        {
            UIElement copyCodeButton;
            UIElement saveLayoutButton;
            UIElement loadLayoutButton;

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
                        copyCodeButton = UICreator.CreateButton("Copy Code")
                        .SetAbsOffsetsX(10, 10)
                        .SetNormalizedAnchoringX(0,1)
                        .SetNormalizedPositionCenterY(0,0)
                        .SetAbsPositionSizeY(10, 50)
                        ,
                        saveLayoutButton = UICreator.CreateButton("Save Layout")
                        .SetAbsOffsetsX(10, 10)
                        .SetNormalizedAnchoringX(0, 1)
                        .SetNormalizedPositionCenterY(0, 0)
                        .SetAbsPositionSizeY(70, 50)
                        ,
                        loadLayoutButton = UICreator.CreateButton("Load Layout")
                        .SetAbsOffsetsX(10, 10)
                        .SetNormalizedAnchoringX(0, 1)
                        .SetNormalizedPositionCenterY(0, 0)
                        .SetAbsPositionSizeY(130, 50)
                        ,
                        _propertiesPanel = UICreator.CreatePanel(new Color4(1))
                        .SetAbsoluteOffset(10)
                        .AddComponent(
                            new UIEdgeSnapConstraint(loadLayoutButton, UIRectEdgeSnapEdge.Bottom, UIRectEdgeSnapEdge.Top)
                        )
                        .AddChildren(
                            UICreator.CreateUIElement(
                                new UIText("Properties", new Color4(0))
                            )
                            .SetAbsOffsetsX(10,10)
                            .SetNormalizedAnchoringX(0,1)
                            .SetNormalizedPositionCenterY(1,1)
                            .SetAbsPositionSizeY(-10, 50)
                        )
                    )
                    //_propertiesView = new UILinearArrangement(vertical: true, reverse: false, padding: 10f)
                )
            );

            _root.GetComponentOfType<UIMouseListener>().OnMousePressed += OnWindowClicked;
        }

        private void InitRightclickMenu()
        {
            UIElement createButton;
            UIElement deleteButton;

            _rightclickMenu = UICreator.CreateUIElement(
                new UILinearArrangement(true, false, 10)
            )
            .SetNormalizedPositionCenter(0, 0, 0, 0)
            .SetAbsPositionSize(0, 0, 200, 10 + (16 + 10) * 2)
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
