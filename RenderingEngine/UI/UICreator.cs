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
                new UIRect(new Color4(0,0), color, thickness)
            );
        }

        /// <summary>
        /// Creates a container with a child at the top that has a fixed height,
        /// and a child at the bottom that has a fixed top offset.
        /// equivelant to:
        /// <c>
        /// CreateUIElement()
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
        /// <param name="offset"></param>
        /// <param name="upper"></param>
        /// <param name="lower"></param>
        /// <returns></returns>
        public static UIElement TopSplit(float offset, UIElement upper, UIElement lower)
        {
            return CreateUIElement()
                .AddChildren(
                    CreateUIElement()
                    .AnchoredPosCenterY(1,1)
                    .PosSizeY(0,offset)
                    .AddChildren(
                        upper
                    )
                    ,
                    CreateUIElement()
                    .OffsetsY(0, offset)
                    .AddChildren(
                        lower
                    )
                );
        }

        /// <summary>
        /// Same as TopSplit but from the bottom
        /// </summary>
        public static UIElement BottomSplit(float offset, UIElement upper, UIElement lower)
        {
            return CreateUIElement()
                .AddChildren(
                    CreateUIElement()
                    .AnchoredPosCenterY(0, 0)
                    .PosSizeY(0, offset)
                    .AddChildren(
                        lower
                    )
                    ,
                    CreateUIElement()
                    .OffsetsY(offset, 0)
                    .AddChildren(
                        upper
                    )
                );
        }

        /// <summary>
        /// Same as TopSplit but from the right
        /// </summary>
        public static UIElement RightSplit(float offset, UIElement left, UIElement right)
        {
            return CreateUIElement()
                .AddChildren(
                    CreateUIElement()
                    .OffsetsX(0, offset)
                    .AddChildren(
                        left
                    )
                    ,
                    CreateUIElement()
                    .AnchoredPosCenterX(1, 1)
                    .PosSizeX(0, offset)
                    .AddChildren(
                        right
                    )
                );
        }

        /// <summary>
        /// Same as TopSplit but from the left
        /// </summary>
        public static UIElement LeftSplit(float offset, UIElement left, UIElement right)
        {
            return CreateUIElement()
                .AddChildren(
                    CreateUIElement()
                    .AnchoredPosCenterX(0,0)
                    .PosSizeX(0,offset)
                    .AddChildren(
                        left
                    )
                    ,
                    CreateUIElement()
                    .OffsetsX(offset, 0)
                    .AddChildren(
                        right
                    )
                );
        }

        /// <summary>
        /// Same as InRows but places elements into columns instead.
        /// </summary>
        /// <param name="columnOffsets">Set to null to get even anchoring</param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static UIElement InColumns(float[] columnOffsets, UIElement[] elements)
        {
            return LinearAnchoring(false, elements, columnOffsets);
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
        public static UIElement InRows(float[] rowOffsets, UIElement[] elements)
        {
            return LinearAnchoring(true, elements, rowOffsets);
        }

        private static UIElement LinearAnchoring(bool vertical, UIElement[] elements, float[] offsets = null)
        {
            float last = 0;
            UIElement root = CreateUIElement();

            for (int i = 0; i < elements.Length; i++)
            {
                float amount;
                if(offsets == null)
                {
                    amount = (i+1.0f) / (float)elements.Length;
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

                root.AddChild(elements[i]);

                last = amount;
            }

            return root;
        }
    }
}
