using RenderingEngine.Datatypes;
using RenderingEngine.Datatypes.UI;
using RenderingEngine.UI;
using RenderingEngine.UI.Components.AutoResizing;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;
using System;
using System.Collections.Generic;
using System.Text;
using UICodeGenerator.DraggableRect;

namespace UICodeGenerator.ComponentEditors
{
    public class AllComponentsDropdownList
    {
        public UIElement Root;

        public AllComponentsDropdownList()
        {
            InitializeAllEditors();
        }

        private void InitializeAllEditors()
        {
            Root = UICreator.CreateUIElement(
                new UIRect(new Color4(0,0), new Color4(0,1), 1),
                new UILinearArrangement(true,false, 30, 5)
            );

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
                        string name = type.AssemblyQualifiedName;

                        UIElement button = UICreator.CreateButton(
                            name, "Consolas", 11
                        );

                        button.GetComponentOfType<UIText>().HorizontalAlignment = HorizontalAlignment.Left;


                        Root.AddChild(button);
                    }
                }
            }
        }
    }
}
