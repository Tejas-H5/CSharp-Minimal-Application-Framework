using RenderingEngine.Datatypes;
using RenderingEngine.Datatypes.UI;
using RenderingEngine.UI;
using RenderingEngine.UI.Components.AutoResizing;
using RenderingEngine.UI.Components.MouseInput;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UICodeGenerator.DraggableRect;

namespace UICodeGenerator.ComponentEditors
{
    class NameComponentPair
    {
        public string Name;
        public Type ComponentType;

        public NameComponentPair(string name, Type component)
        {
            Name = name;
            ComponentType = component;
        }
    }

    public class AllComponentsDropdownList
    {
        public UIElement Root;

        Dictionary<string, List<NameComponentPair>> _assemblySeperatedComponents = new Dictionary<string, List<NameComponentPair>>();

        DraggableRectSelectedState _state;

        public AllComponentsDropdownList(DraggableRectSelectedState state)
        {
            _state = state;

            InitializeAllEditors();
        }

        private void InitializeAllEditors()
        {
            Root = UICreator.CreateUIElement(
                new UIRect(new Color4(0, 0), new Color4(0, 1), 1),
                new UILinearArrangement(true, false, -1, 10)
            );

            FindAllAvailableComponents();

            foreach (var keyValuePair in _assemblySeperatedComponents)
            {
                string assemblyName = keyValuePair.Key;
                List<NameComponentPair> nameComponentPairs = keyValuePair.Value;

                UIElement collapseUncollapseButton;
                UIElement accordionContainer;

                UIElement accordionRoot = UICreator.CreateUIElement(
                    new UIRect(new Color4(0, 0), new Color4(0, 1), 1),
                    new UILinearArrangement(true, false, -1, 10)
                )
                .AddChildren(
                    collapseUncollapseButton = UICreator.CreateButton(
                        assemblyName, "Consolas", 14
                    )
                    ,
                    accordionContainer = UICreator.CreateUIElement(
                        new UIRect(new Color4(0, 0), new Color4(0, 1), 1),
                        new UILinearArrangement(true, false, 30, 10)
                    )
                );

                for (int i =0; i < nameComponentPairs.Count; i++)
                {
                    string name = nameComponentPairs[i].Name;
                    Type type = nameComponentPairs[i].ComponentType;

                    UIElement button = UICreator.CreateButton(
                        name, "Consolas", 11
                    );

                    button.GetComponentOfType<UIMouseListener>().OnMousePressed += () => {
                        _state.SelectedEditorRect.AddComponent(
                            //Create a component of type 'type' using reflection, then pass it here
                        );
                    };

                    accordionContainer.AddChild(button);
                }

                collapseUncollapseButton.GetComponentOfType<UIMouseListener>().OnMousePressed += () => {
                    accordionContainer.IsVisible = !accordionContainer.IsVisible;
                };
            }
        }

        private void FindAllAvailableComponents()
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
                        string name = type.Name;
                        string nameSpace = type.Namespace;

                        if (!_assemblySeperatedComponents.ContainsKey(nameSpace))
                        {
                            _assemblySeperatedComponents.Add(nameSpace, new List<NameComponentPair>());
                        }

                        _assemblySeperatedComponents[nameSpace].Add(
                            new NameComponentPair(name, type)
                        );
                    }
                }
            }
        }
    }
}
