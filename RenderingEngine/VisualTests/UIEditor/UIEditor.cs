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
        UIElement _uiView;

        UIElement _domRoot;
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
            CTX.SetCurrentFont("Consolas", 16);

            UICreator.Debug = true;
            _selectedState = new DraggableRectSelectedState();

            //Change this to a UIRect editable UI thinggy
            _domRoot = UIDraggableRect.CreateDraggableRect(_selectedState);

            _root = new UIZStack();
            
            _root
            .AddComponent(new UIRectHitbox())
            .AddComponent(new UIMouseListener())
            .AddChildren(
                _mainWorkspace = UICreator.CreateUIElement()
                .AddChildren(
                  _uiView = UICreator.CreateUIElement()
                  .SetNormalizedAnchoring(new Rect2D(0, 0, 0.66f, 1f))
                  .SetAbsoluteOffset(10)
                  .AddChildren(
                      _domRoot
                    )
                  ,
                  _domView = UICreator.CreateUIElement()
                  .SetNormalizedAnchoring(new Rect2D(0.66f, 0, 1f, 1f))
                  .SetAbsoluteOffset(10)
                )
            );

            _root.GetComponentOfType<UIMouseListener>().OnMousePressed += OnWindowClicked;
        }

        private void OnWindowClicked()
        {
            Console.WriteLine("Window clicked");

            _selectedState.SelectedRect = null;
        }

        public override void Render(double deltaTime)
        {
            _root.Draw(deltaTime);
        }


        public override void Update(double deltaTime)
        {
            _root.Update(deltaTime);
        }

        public override void Resize()
        {
            base.Resize();

            _root.Resize();
        }
    }
}
