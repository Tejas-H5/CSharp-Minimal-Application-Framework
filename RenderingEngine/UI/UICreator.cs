using RenderingEngine.Datatypes;
using RenderingEngine.Datatypes.UI;
using RenderingEngine.UI.Components.MouseInput;
using RenderingEngine.UI.Components.Visuals;
using RenderingEngine.UI.Core;

namespace RenderingEngine.UI
{
    public static class UICreator
    {

#if DEBUG
        public static bool Debug = false;
#endif

        public static UIElement CreateUIElement(params UIComponent[] components)
        {
            UIElement root = new UIElement();

            for (int i = 0; i < components.Length; i++)
            {
                root.AddComponent(components[i]);
            }

            return root;
        }

        public static UIElement CreateButton(string buttontext, string fontName = "", int fontSize = -1)
        {
            return CreateButton(buttontext, fontName, fontSize, new Color4(0), new Color4(1f), new Color4(.7f), new Color4(1.0f));
        }

        public static UIElement CreateButton(string buttontext, string fontName, int fontSize, Color4 textColor, Color4 color, Color4 hoverColor, Color4 clickColor)
        {
            return CreateUIElement(
                new UIRect(color),
                new UIRectHitbox(false),
                new UIMouseListener(),
                new UIMouseFeedback(hoverColor, clickColor),
                new UIText(buttontext, textColor, fontName, fontSize, VerticalAlignment.Center, HorizontalAlignment.Center)
                );
        }

        public static UIElement CreateColoredRect(Color4 color)
        {
            return CreateUIElement(
                new UIRect(color)
                );
        }

        public static UIElement CreatePanel(Color4 color)
        {
            return CreatePanel(color, color, color);
        }

        public static UIElement CreatePanel(Color4 color, Color4 hoverColor, Color4 clickedColor)
        {
            return CreateUIElement(
                new UIRect(color),
                new UIRectHitbox(false),
                new UIMouseListener(),
                new UIMouseFeedback(hoverColor, clickedColor)
                );
        }
    }
}
