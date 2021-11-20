namespace MinimalAF
{
	/// <summary>
	/// See <see cref="ElementTransformExtensions"/> to know why these are extension methods rather than normal methods.
	/// </summary>
	public static class ElementContainerExtensions
    {

        /// <summary>
        /// Creates a container with a child at the top that has a fixed height,
        /// and a child at the bottom that has a fixed top offset.
        /// equivelant to:
        /// <c>
        /// baseElement
        ///     .AddChildren(
        ///          CreateElement()
        ///           .AnchoredPosCenterY(1,1)
        ///           .PosSizeY(0, offset)
        ///           .AddChildren(
        ///              upper
        ///          )
        ///            ,
        ///          CreateElement()
        ///            .OffsetsY(0, offset)
        ///            .AddChildren(
        ///                lower
        ///            )
        ///         );

        /// </c>
        /// </summary>
        public static T TopSplit<T>(this T baseElement, float offset, Element upper, Element lower) where T : Element
        {
            baseElement.SetChildren(
                upper
                .AnchoredPosCenterY(1, 1)
                .PosSizeY(0, offset)
                ,
                lower
                .OffsetsY(0, offset)
            );


            return baseElement;
        }

        /// <summary>
        /// Same as TopSplit but from the bottom
        /// </summary>
        public static T BottomSplit<T>(this T baseElement, float offset, Element upper, Element lower) where T : Element
        {
            baseElement.SetChildren(
                    lower
                    .AnchoredPosCenterY(0, 0)
                    .PosSizeY(0, offset)

                    ,
                    upper
                    .OffsetsY(offset, 0)
                );

            return baseElement;

            return baseElement;
        }

        /// <summary>
        /// Same as TopSplit but from the right
        /// </summary>
        public static T RightSplit<T>(this T baseElement, float offset, Element left, Element right) where T : Element
        {
            baseElement.SetChildren(
                    left
                    .OffsetsX(0, offset)
                    ,
                    right
                    .AnchoredPosCenterX(1, 1)
                    .PosSizeX(0, offset)
                );

            return baseElement;
        }

        /// <summary>
        /// Same as TopSplit but from the left
        /// </summary>
        public static T LeftSplit<T>(this T baseElement, float offset, Element left, Element right) where T : Element
        {
            baseElement.SetChildren(
                    left
                    .AnchoredPosCenterX(0, 0)
                    .PosSizeX(0, offset)
                    ,
                    right
                    .OffsetsX(offset, 0)
                );

            return baseElement;
        }

        /// <summary>
        /// Same as InRows but places elements into columns instead.
        /// </summary>
        /// <param name="columnOffsets">Set to null to get even anchoring</param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static T InColumns<T>(this T baseElement, float[] columnOffsets, params Element[] elements) where T:Element
        {
            return LinearAnchoring(baseElement, false, elements, columnOffsets);
        }

        /// <summary>
        /// places elements[i] in container with Y anchoring of rowOffsets[i-1], rowOffsets[i]
        /// and an X anchoring of 0,1.
        /// 
        /// (Overwrites all existing children)
        /// 
        /// These can be thought of as rows.
        /// 
        /// The same effect can also be achieved by typing code manually but that is tedious
        /// </summary>
        /// <param name="rowOffsets">Set to null to get even anchoring</param>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static T InRows<T>(this T baseElement, float[] rowOffsets, params Element[] elements) where T : Element
        {
            return LinearAnchoring(baseElement, true, elements, rowOffsets);
        }

        private static T LinearAnchoring<T>(this T baseElement, bool vertical, Element[] elements, float[] offsets = null) where T : Element
        {
            float previousAnchor = 0;

            for (int i = 0; i < elements.Length; i++)
            {
                float rightOrTopAnchor;
                if (offsets == null)
                {
                    rightOrTopAnchor = (i + 1.0f) / elements.Length;
                }
                else
                {
                    if (i == elements.Length - 1)
                    {
                        rightOrTopAnchor = 1;
                    }
                    else
                    {
                        rightOrTopAnchor = offsets[i];
                    }
                }

                if (vertical)
                {
                    elements[i].AnchorsY(previousAnchor, rightOrTopAnchor);
                }
                else
                {
                    elements[i].AnchorsX(previousAnchor, rightOrTopAnchor);
                }

                previousAnchor = rightOrTopAnchor;
            }

            baseElement.SetChildren(elements);

            return baseElement;
        }

        public static T SetChildren<T>(this T baseElement, params Element[] children) where T : Element
        {
            baseElement.Children = children;

            return baseElement;
        }
    }
}
