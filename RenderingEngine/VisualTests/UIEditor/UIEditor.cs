using RenderingEngine.Datatypes;
using RenderingEngine.Logic;
using RenderingEngine.Rendering;
using RenderingEngine.UI;
using RenderingEngine.UI.Components;
using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace RenderingEngine.VisualTests.UIEditor
{
    public class UIEditor : EntryPoint
    {
        UIZStack _root;
        UIElement _mainWorkspace;
        UIElement _domView;
        UIElement _propertiesView;
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


            UIElement createButton;
            UIElement deleteButton;
            UIElement togglePosSizeXButton;
            UIElement togglePosSizeYButton;

            _domRoot = UIDraggableRect.CreateDraggableRect(_selectedState);
            _domRootRect = _domRoot.GetComponentOfType<UIDraggableRect>();

            InitRightclickMenu(out createButton, out deleteButton, out togglePosSizeXButton, out togglePosSizeYButton);
            InitRootUI();
        }

        private void InitRootUI()
        {
            _root = new UIZStack();
            _root.AddComponent(new UIRectHitbox())
            .AddComponent(new UIMouseListener())
            .AddChildren(
                _mainWorkspace = UICreator.CreateUIElement()
                .AddChildren(
                    _uiView = UICreator.CreateUIElement(
                        new UIInverseStencil()
                    )
                    .SetNormalizedAnchoring(new Rect2D(0, 0, 0.66f, 1f))
                    .SetAbsoluteOffset(10)
                    .AddChildren(
                        _domRoot
                    )
                    ,
                    _domView = UICreator.CreateUIElement()
                    .SetNormalizedAnchoring(new Rect2D(0.66f, 0.5f, 1f, 1f))
                    .SetAbsoluteOffset(10)
                    ,
                    _propertiesView = new UILinearArrangement(vertical: true, reverse: false, padding: 10f)
                )
            );

            _root.GetComponentOfType<UIMouseListener>().OnMousePressed += OnWindowClicked;
        }

        private void InitRightclickMenu(out UIElement createButton, out UIElement deleteButton, out UIElement togglePosSizeXButton, out UIElement togglePosSizeYButton)
        {
            _rightclickMenu = new UILinearArrangement(true, false, 10)
                .SetNormalizedPositionCenter(0, 0, 0, 0)
                .SetAbsPositionSize(0, 0, 100, 10 + (14+10)*4)
                .AddChildren(
                    createButton = UICreator.CreateButton("Add"),
                    deleteButton = UICreator.CreateButton("Delete"),
                    togglePosSizeXButton = UICreator.CreateButton("toggle xAnchor"),
                    togglePosSizeYButton = UICreator.CreateButton("toggle yAnchors")
                );
            _rightclickMenu.IsVisible = false;
            createButton.GetComponentOfType<UIMouseListener>().OnMousePressed += OnNewButtonClicked;
            deleteButton.GetComponentOfType<UIMouseListener>().OnMousePressed += OnDeleteButtonClicked;
            togglePosSizeXButton.GetComponentOfType<UIMouseListener>().OnMousePressed += OnTogglePosSizeXButtonClicked;
            togglePosSizeYButton.GetComponentOfType<UIMouseListener>().OnMousePressed += OnTogglePosSizeYButtonClicked;
        }

        private void OnTogglePosSizeXButtonClicked()
        {
            if (_selectedState.SelectedRect == null)
                return;
            _selectedState.SelectedRect.ToggleXAnchoring();
        }

        private void OnTogglePosSizeYButtonClicked()
        {
            if (_selectedState.SelectedRect == null)
                return;
            _selectedState.SelectedRect.ToggleYAnchoring();
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
                    _rightclickMenu.IsVisible = false;
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
