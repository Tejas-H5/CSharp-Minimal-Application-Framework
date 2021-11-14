using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;
using MinimalAF.UI;
using System;

namespace MinimalAF.VisualTests.UI
{
    public class UIAspectRatioTest : Container
    {
        Element _root;
        Element _textInputElement;

        public UIAspectRatioTest()
            : base()
        {
            this.SetChildren(
                new OutlineRect(new Color4(1, 1, 1, 1),2)
                .Offsets(10)
                .SetChildren(
                    new AspectRatioElement(
                        4f / 3f,
                        new OutlineRect(
                            new Color4(1, 0, 0, 1),
                            1,
                            new TextElement("4 : 3 Aspect ratio", new Color4(1, 0, 0, 1))
                        )
                    )
                    .Offsets(10)
                )
            );
        }

        public override void OnStart()
        {
            Window w = GetAncestor<Window>();
            w.Size = (800, 600);
            w.Title = "UIAspectRatioTest";

            w.RenderFrequency = 120;
            w.UpdateFrequency = 120; 

            CTX.SetClearColor(0, 0, 0, 0);
        }
    }
}
