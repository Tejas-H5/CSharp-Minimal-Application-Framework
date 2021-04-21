using RenderingEngine.Datatypes;
using RenderingEngine.Datatypes.UI;
using RenderingEngine.UI;
using RenderingEngine.UI.Components.AutoResizing;
using RenderingEngine.UI.Components.MouseInput;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UICodeGenerator.DraggableRect;

namespace UICodeGenerator.ComponentEditors
{
    public class ComponentEditorListUI
    {
        DraggableRectSelectedState _state;
        UIElement _root;

        public UIElement Root { get { return _root; } }

        Dictionary<Type, IComponentEditorUI> _componentEditors = new Dictionary<Type, IComponentEditorUI>();
        private UIElement _nullComponent;
        private UIElement _emptySpace;
        private UIElement _addComponentButton;

        public ComponentEditorListUI(DraggableRectSelectedState state)
        {
            _root = UICreator.CreateUIElement(
                new UILinearArrangement(vertical: true, reverse: false, elementSizing: -1, padding: 10)
            );

            _state = state;
            _state.OnSelectionChanged += OnSelectionChanged;

            InitializeAllEditors();

            InitNullComponent();

            InitAddComponentButton();
        }


        private void InitAddComponentButton()
        {
            _addComponentButton = UICreator.CreateButton("Add Component", "Consolas", 12)
            .SetAbsOffsetsX(40, 40)
            .SetNormalizedPositionCenterY(1, 1)
            .SetAbsPositionSizeY(-10, 60);

            _addComponentButton.GetComponentOfType<UIMouseListener>().OnMousePressed += OnAddComponentButtonPressed;
        }

        private void OnAddComponentButtonPressed()
        {
            
        }

        private void InitNullComponent()
        {
            _nullComponent = UICreator.CreateUIElement(
                new UIText("No object selected", new Color4(0, 0, 0, 1), VerticalAlignment.Center, HorizontalAlignment.Center)
            )
            .SetNormalizedPositionCenterY(0.5f, 0.5f)
            .SetAbsPositionSizeY(0, 40)
            .SetNormalizedAnchoringX(0, 1)
            .SetAbsOffsetsX(30, 30);
        }

        private void InitializeAllEditors()
        {
            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in asm.GetTypes())
                {
                    if (type == typeof(UIDraggableRect))
                        continue;

                    if (type.IsAbstract)
                        continue;

                    if (type.IsSubclassOf(typeof(UIComponent)))
                    {

                        Type emptyType = typeof(ComponentEditorUI<>);
                        Type[] typeArgs = { type };
                        object[] args = { null, false };

                        Type genericType = emptyType.MakeGenericType(typeArgs);
                        IComponentEditorUI editor = (IComponentEditorUI)Activator.CreateInstance(genericType, args);

                        editor.Root
                        .SetNormalizedPositionCenterY(1, 1);

                        _componentEditors[type] = editor;
                    }
                }
            }
        }

        private void OnSelectionChanged(UIDraggableRect obj)
        {
            _root.RemoveAllChildren();

            if(obj == null)
            {
                _root.AddChild(_nullComponent);
                return;
            }

            foreach(UIComponent c in obj.Parent.Components)
            {

                Type t = c.GetType();
                if (t == typeof(UIDraggableRect))
                    continue;

                IComponentEditorUI editor = _componentEditors[t];
                _root.AddChild(editor.Root);
                editor.Bind(c);
            }

            _root.AddChild(_addComponentButton);
        }
    }
}
