using MinimalAF.Datatypes;
using MinimalAF.UI;
using MinimalAF.UI;

namespace MinimalAF.UI
{
    public static class UICreator
    {

#if DEBUG
        public static bool Debug = false;
#endif

        public static UIElement CreateUIElement(params UIComponent[] components)
        {
            return new UIElement()
                .AddComponents(components);
        }

        public static UIElement CreateButton(string buttontext, string fontName = "", int fontSize = -1)
        {
            return CreateButton(buttontext, fontName, fontSize, new Color4(0), new Color4(1f), new Color4(.7f), new Color4(1.0f));
        }

        public static UIElement CreateButton(string buttontext, string fontName, int fontSize, Color4 textColor, Color4 color, Color4 hoverColor, Color4 clickColor)
        {
            return CreateUIElement(
                new UIRect(color),
                new UIRectHitbox(),
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

        public static UIElement CreateRectOutline(Color4 color, int thickness = 1)
        {
            return CreateUIElement(
                new UIRect(new Color4(0, 0), color, thickness)
            );
        }

        /// <summary>
        /// Creates a container with a child at the top that has a fixed height,
        /// and a child at the bottom that has a fixed top offset.
        /// equivelant to:
        /// <c>
        /// baseElement
        ///     .AddChildren(
        ///          CreateUIElement()
        ///           .AnchoredPosCenterY(1,1)
        ///           .PosSizeY(0, offset)
        ///           .AddChildren(
        ///              upper
        ///          )
        ///            ,
        ///          CreateUIElement()
        ///            .OffsetsY(0, offset)
        ///            .AddChildren(
        ///                lower
        ///            )
        ///         );
        /// </c>
        /// </summary>
        public static UIElement TopSplit(this UIElement baseElement, float offset, UIElement upper, UIElement lower)
        {
            return baseElement
                .AddChildren(
                    upper
                    .AnchoredPosCenterY(1, 1)
                    .PosSizeY(0, offset)
                    ,
                    lower
                    .OffsetsY(0, offset)
                );
        }

        /// <summary>
        /// Same as TopSplit but from the bottom
        /// </summary>
        public static UIElement BottomSplit(this UIElement baseElement, float offset, UIElement upper, UIElement lower)
        {
            return baseElement
                .AddChildren(
                    lower
                    .AnchoredPosCenterY(0, 0)
                    .PosSizeY(0, offset)

                    ,
                    upper
                    .OffsetsY(offset, 0)
                );
        }

        /// <summary>
        /// Same as TopSplit but from the right
        /// </summary>
        public static UIElement RightSplit(this UIElement baseElement, float offset, UIElement left, UIElement right)
        {
            return baseElement
                .AddChildren(
                    left
                    .OffsetsX(0, offset)
                    ,
                    right
                    .AnchoredPosCenterX(1, 1)
                    .PosSizeX(0, offset)
                );
        }

        /// <summary>
        /// Same as TopSplit but from the left
        /// </summary>
        public static UIElement LeftSplit(this UIElement baseElement, float offset, UIElement left, UIElement right)
        {
            return baseElement
                .AddChildren(
                    left
                    .AnchoredPosCenterX(0, 0)
                    .PosSizeX(0, offset)
                    ,
                    right
                    .OffsetsX(offset, 0)
                );
        }

        /// <summary>
        /// Same as InRows but places elements into columns instead.
        /// </summary>
        /// <param name="columnOffsets">Set to null to get even anchoring</param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static UIElement InColumns(this UIElement baseElement, float[] columnOffsets, UIElement[] elements)
        {
            return baseElement.LinearAnchoring(false, elements, columnOffsets);
        }

        /// <summary>
        /// places elements[i] in container with Y anchoring of rowOffsets[i-1], rowOffsets[i]
        /// and an X anchoring of 0,1.
        /// 
        /// These can be thought of as rows.
        /// 
        /// The same effect can also be achieved by typing code manually but that is tedious
        /// </summary>
        /// <param name="rowOffsets">Set to null to get even anchoring</param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static UIElement InRows(this UIElement baseElement, float[] rowOffsets, UIElement[] elements)
        {
            return baseElement.LinearAnchoring(true, elements, rowOffsets);
        }

        private static UIElement LinearAnchoring(this UIElement baseElement, bool vertical, UIElement[] elements, float[] offsets = null)
        {
            float last = 0;

            for (int i = 0; i < elements.Length; i++)
            {
                float amount;
                if (offsets == null)
                {
                    amount = (i + 1.0f) / elements.Length;
                }
                else
                {
                    amount = offsets[i];
                }

                if (vertical)
                {
                    elements[i].AnchorsY(last, amount);
                }
                else
                {
                    elements[i].AnchorsX(last, amount);
                }

                baseElement.AddChild(elements[i]);

                last = amount;
            }

            return baseElement;
        }
    }
}
