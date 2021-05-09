using RenderingEngine.Datatypes;
using RenderingEngine.Datatypes.Geometric;
using RenderingEngine.Datatypes.UI;
using RenderingEngine.Logic;
using RenderingEngine.UI;
using RenderingEngine.UI.Components.AutoResizing;
using RenderingEngine.UI.Components.MouseInput;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
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
        private UIElement _addComponentButtonAndList;
        private UIElement _addComponentsUI;

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


        UIMouseListener _mouseListener;
        private void InitAddComponentButton()
        {
            UIElement addComponentButton;
            UIMouseScroll scrollComponent;
            UIElement scrollTarget;

            AllComponentsDropdownList componentList = new AllComponentsDropdownList(_state);

            componentList.OnComponentAdded += ComponentList_OnComponentAdded;

            _addComponentButtonAndList = UICreator.CreateUIElement(
                new UIRect(new Color4(0,0), new Color4(0,1),1),
				new UILinearArrangement(true, false, -1, 10),
                new UIRectHitbox(),
                _mouseListener = new UIMouseListener()
            )
            .SetAbsOffsetsX(10, 40)
            .SetNormalizedPositionCenterY(1, 1)
            .SetAbsPositionSizeY(-10, 60)
            .AddChildren(
                addComponentButton = UICreator.CreateButton(
                "Add Component", "Consolas", 12, new Color4(0, 1),
                new Color4(0, 0), new Color4(1.0f, 0.4f), new Color4(1, 1)
                )
                .SetAbsOffsetsX(10, 10)
                .SetNormalizedPositionCenterY(1, 1)
                .SetAbsPositionSizeY(-10, 60)
                ,
                _addComponentsUI = UICreator.CreateUIElement(
                    new UIInverseStencil(),
                    new UIRectHitbox(),
                    new UIMouseListener(),
                    scrollComponent = new UIMouseScroll(vertical:true)
                    //,new UIRect(new Color4(1,0,0,1), new Color4(1,0,0,1), 1)
                )
                .SetAbsOffsetsX(10, 10)
                .SetNormalizedPositionCenterY(1, 1)
                .SetAbsPositionSizeY(-80, 300)
                .AddChildren(
                    scrollTarget = componentList.Root
                    .SetNormalizedPositionCenterY(1, 1)
                )
            );

            addComponentButton.GetComponentOfType<UIMouseListener>().OnMousePressed += OnAddComponentButtonPressed;

            _addComponentsUI.SetNormalizedCenter(0, 1);
            _addComponentsUI.IsVisible = false;

            scrollComponent.Target = scrollTarget;
        }

        private void ComponentList_OnComponentAdded()
        {
            OnSelectionChanged(_state.SelectedEditorRect);
            _addComponentsUI.IsVisible = false;
        }

        private void OnAddComponentButtonPressed(MouseEventArgs e)
        {
            _addComponentsUI.IsVisible = !_addComponentsUI.IsVisible;
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
                    if (type == typeof(UIElementEditor))
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

        private void OnSelectionChanged(UIElementEditor obj)
        {
            _root.RemoveAllChildren();

            if(obj == null)
            {
                _root.AddChild(_nullComponent);
                return;
            }

            foreach(UIComponent c in obj.Components)
            {
                Type t = c.GetType();
                IComponentEditorUI editor = _componentEditors[t];
                _root.AddChild(editor.Root);
                editor.Bind(c);
            }

            _root.AddChild(_addComponentButtonAndList);
        }
    }
}
