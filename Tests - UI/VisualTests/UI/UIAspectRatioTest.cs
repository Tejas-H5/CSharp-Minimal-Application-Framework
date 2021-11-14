using MinimalAF.Datatypes;
using MinimalAF;
using MinimalAF.Rendering;
using MinimalAF;
using System;

namespace MinimalAF.VisualTests.UI
{
    public class UIAspectRatioTest : Container
    {
        public UIAspectRatioTest()
            : base()
        {
            this.SetChildren(
                new OutlineRect(new Color4(1, 1, 1, 1),2)
                .Offsets(10)
                .SetChildren(
                    new AspectRatioElement(4f / 3f)
                    .Offsets(10)
                    .AnchoredCenter(0.5f,0.5f)
                    .SetChildren(
                        new OutlineRect(new Color4(1, 0, 0, 1),1)
                        .SetChildren(
                            new TextElement("4 : 3 Aspect ratio", new Color4(1, 0, 0, 1))
                        )
                    )
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
